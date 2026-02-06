using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Server.Gravity;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Radio.EntitySystems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Shared._Adventure.EnergyCores;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Database;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.DoAfter;
using Content.Shared.Gravity;
using Content.Shared.NodeContainer;
using Content.Shared.Radio;
using Content.Shared.UserInterface;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Adventure.EnergyCores;

public sealed partial class EnergyCoreSystem : EntitySystem
{
    [Dependency] private readonly GasVentScrubberSystem _scrubberSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IEntityManager _e = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly Shared.Damage.Systems.DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly GravitySystem _gravitySystem = default!;
    [Dependency] private readonly ThrusterSystem _thrusterSystem = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    private EntityQuery<PowerSupplierComponent> _recQuery;
    private TimeSpan _nextTickCore = TimeSpan.FromSeconds(1);

    public override void Initialize()
    {
        SubscribeLocalEvent<EnergyCoreComponent, MapInitEvent>(OnMapInit);
        _recQuery = GetEntityQuery<PowerSupplierComponent>();
        SubscribeLocalEvent<HeatFreezingCoreComponent, AtmosDeviceUpdateEvent>(OnDeviceUpdated);
        SubscribeLocalEvent<EnergyCoreComponent, TogglePowerDoAfterEvent>(TogglePowerDoAfter);
        SubscribeLocalEvent<EnergyCoreConsoleComponent, NewLinkEvent>(OnNewLink);
        SubscribeLocalEvent<EnergyCoreConsoleComponent, UserOpenActivatableUIAttemptEvent>(OnTryOpenUI);
        SubscribeLocalEvent<EnergyCoreConsoleComponent, EnergyCoreConsoleIsOnMessage>(OnPowerToggled);
        SubscribeLocalEvent<EnergyCoreComponent, EntParentChangedMessage>(OnParentChanged);
    }
    private void OnMapInit(EntityUid uid, EnergyCoreComponent component, MapInitEvent args)
    {
        component.ForceDisabled = true;
        TogglePowerDiscrete(uid, core: component);
        component.TimeOfLife = 0;
        if (!TryComp(uid, out HeatFreezingCoreComponent? heatFreezing)) return;
        heatFreezing.FilterGases.Add(heatFreezing.AbsorbGas);
    }

    private void OnDeviceUpdated(EntityUid uid, HeatFreezingCoreComponent component, ref AtmosDeviceUpdateEvent args)
    {
        var timeDelta = args.dt;
        // If we are on top of a connector port, empty into it.
        if (!_nodeContainer.TryGetNode(uid, component.PortName, out PipeNode? portableNode))
            return;
        if (args.Grid is not { } grid)
            return;
        if (!TryComp(uid, out EnergyCoreComponent? core)) return;

        var position = _transformSystem.GetGridTilePositionOrDefault(uid);
        var environment = _atmosphereSystem.GetTileMixture(grid, args.Map, position, true);
        if (environment == null) return;
        // widenet
        var enumerator = _atmosphereSystem.GetAdjacentTileMixtures(grid, position, false, true);
        while (enumerator.MoveNext(out var adjacent))
        {
            if (core.TimeOfLife <= 2600)
            {
                Scrub(timeDelta, portableNode, adjacent, component);
                core.TimeOfLife += portableNode.Air.GetMoles(component.AbsorbGas) * core.SecPerMoles;
                core.TimeOfLife = Math.Min(core.TimeOfLife, 2700);
                portableNode.Air.Clear();
            }

            if (core.TimeOfLife > 600)
            {
                core.LowTimeWarningSent = false;
            }
            else if (core.TimeOfLife < 600 && !core.LowTimeWarningSent)
            {
                var message = Loc.GetString("energy-core-low-time-warning");
                _radio.SendRadioMessage(uid, message,
                    _prototype.Index<RadioChannelPrototype>(core.EngineeringChannel),
                    uid);
                core.LowTimeWarningSent = true;
            }

            if (core.Working && !core.ForceDisabled)
            {
                if (core.Size == 2)
                    _atmosphereSystem.AddHeat(environment, 2000);
                else if (core.Size == 3)
                    _atmosphereSystem.AddHeat(environment, 4000);
            }

            if (core.TimeOfLife > 0 && core.ForceDisabled)
                core.ForceDisabled = false;
            if (adjacent.Temperature >= 750)
            {
                if (!core.Overheat)
                {
                    OverHeating(uid, core);
                }
            }
            else if (core.Overheat)
            {
                core.Overheat = false;
                core.OverheatSent = false;
            }
        }
    }

    private bool Scrub(float timeDelta, PipeNode scrubber, GasMixture tile, HeatFreezingCoreComponent target)
    {
        if (tile.Temperature > target.FilterTemperature) return false;
        return _scrubberSystem.Scrub(timeDelta, target.TransferRate * _atmosphereSystem.PumpSpeedup(), ScrubberPumpDirection.Scrubbing, target.FilterGases, tile, scrubber.Air);
    }
    private void Pump(GasMixture? enviroment, PipeNode pipe, HeatFreezingCoreComponent target)
    {
        if (enviroment == null || pipe == null) return;
        _atmosphereSystem.Merge(enviroment, pipe.Air.Remove(target.TransferRate * _atmosphereSystem.PumpSpeedup()));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_timing.CurTime > _nextTickCore)
        {
            EnergyCoreTick();
            _nextTickCore += TimeSpan.FromSeconds(1);
        }
    }

    private void OverHeating(EntityUid uid, EnergyCoreComponent component)
    {
        if (!component.OverheatSent)
        {
            component.Overheat = true;
            component.OverheatSent = true;
            var message = Loc.GetString("energy-core-overheat-warning");
            _radio.SendRadioMessage(uid, message,
                _prototype.Index<RadioChannelPrototype>(component.EngineeringChannel), uid);
        }
        _damageable.TryChangeDamage(uid, component.Damage, true);

        var environment = _atmosphereSystem.GetTileMixture(uid);
        if (environment != null)
            environment.Temperature += component.Heating;
    }
    private void Absorb(EntityUid uid, EnergyCoreComponent component, PipeNode air)
    {
        if (!TryComp(uid, out HeatFreezingCoreComponent? heatfreeze)) return;
        if (component.Overheat && component.TimeOfLife > 0)
        {
            ForceTurnOff(uid, component);
        }
    }

    private void ForceTurnOff(EntityUid uid, EnergyCoreComponent component)
    {
        component.Overheat = false;
        component.ForceDisabled = true;
        TogglePower(uid);
    }

    private void Working(EntityUid uid, EnergyCoreComponent component, PipeNode air)
    {
        Absorb(uid, component, air);
        var pos = Transform(uid);
        var environment = _atmosphereSystem.GetTileMixture(pos.GridUid, pos.MapUid, _transformSystem.GetGridTilePositionOrDefault(uid), true);
        if (component.Working && !component.ForceDisabled && environment != null)
        {
            if (component.TimeOfLife > component.LifeAfterOverheat)
            {
                component.TimeOfLife -= 1;
                if (component.TimeOfLife <= 0 && !component.isUndead)
                    OverHeating(uid, component);
            }
            else
            {
                ForceTurnOff(uid, component);
            }
        }
    }
    private void EnergyCoreTick()
    {
        var query = EntityQueryEnumerator<EnergyCoreComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (!TryComp(uid, out DamageableComponent? damage)) return;
            var energyCore = uid;
            var timeOfLife = comp.TimeOfLife;
            var isOn = comp.Working;
            var console = comp.EnergyCoreConsoleEntity;
            var curDamage = damage.TotalDamage.Float();
            if (_timing.CurTime > comp.NextTick)
            {
                if (!TryComp<NodeContainerComponent>(uid, out var component))
                    continue;
                if (!TryComp<HeatFreezingCoreComponent>(uid, out var heatfreeze))
                    continue;
                if (!_nodeContainer.TryGetNode(uid, heatfreeze.PortName, out PipeNode? cur))
                {
                    continue;
                }
                Working(uid, comp, cur);
            }
            if (console is not EntityUid entity) return;
            _ui.SetUiState(entity, EnergyCoreConsoleUiKey.Key, new EnergyCoreConsoleUpdateState(GetNetEntity(energyCore), timeOfLife, isOn, curDamage));
        }
    }

    public void TogglePower(EntityUid uid, bool playSwitchSound = true, EnergyCoreComponent? core = null, EntityUid? user = null)
    {
        if (core == null) if (!TryComp(uid, out core)) return;
        if (core.ForceDisabled) return;
        if (!TryComp(uid, out ApcPowerReceiverComponent? receiver)) return;
        EnergyCoreState dataForSet;
        if (receiver.PowerDisabled)
            dataForSet = EnergyCoreState.Enabling;
        else
            dataForSet = EnergyCoreState.Disabling;
        _appearance.SetData(uid, EnergyCoreVisualLayers.IsOn, dataForSet);
        var time = receiver.PowerDisabled ? core.EnablingLength : core.DisablingLenght;
        _doAfterSystem.TryStartDoAfter(new DoAfterArgs(_e, uid, TimeSpan.FromSeconds(time), new TogglePowerDoAfterEvent(_e.GetNetEntity(user)), uid, target: uid, used: uid));
    }
    private void TogglePowerDoAfter(EntityUid uid, EnergyCoreComponent component, TogglePowerDoAfterEvent args)
    {
        TogglePowerDiscrete(uid, core: component, user: _e.GetEntity(args.Initer));
    }

    private bool TogglePowerDiscrete(EntityUid uid, bool playSwitchSound = true, EnergyCoreComponent? core = null, EntityUid? user = null)
    {
        if (core == null) return true;
        if (!TryComp(uid, out PowerSupplierComponent? supplier)) return true;
        if (!TryComp(uid, out ApcPowerReceiverComponent? receiver)) return true;

        supplier.Enabled = !supplier.Enabled;

        if (supplier.Enabled)
            supplier.MaxSupply = core.BaseSupply;
        else
            supplier.MaxSupply = 0;

        if (!receiver.NeedsPower)
        {
            receiver.PowerDisabled = false;
            return true;
        }
        receiver.PowerDisabled = !receiver.PowerDisabled;

        if (user != null)
            _adminLogger.Add(LogType.Action, LogImpact.Low, $"{ToPrettyString(user.Value):player} hit power button on {ToPrettyString(uid)}, it's now {(!supplier.Enabled ? "on" : "off")}");

        if (playSwitchSound)
        {
            _audio.PlayPvs(new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg"), uid,
                AudioParams.Default.WithVolume(-2f));
        }
        var dataForSet = !receiver.PowerDisabled ? EnergyCoreState.Enabled : EnergyCoreState.Disabled;
        _appearance.SetData(uid, EnergyCoreVisualLayers.IsOn, dataForSet);
        core.Working = !receiver.PowerDisabled;

        if (TryComp(uid, out TransformComponent? xform) &&
            TryComp<GravityComponent>(xform.ParentUid, out var gravity))
        {
            if (core.Working)
            {
                _gravitySystem.EnableGravity(xform.ParentUid, gravity);
            }
            else
            {
                _gravitySystem.RefreshGravity(xform.ParentUid, gravity);
            }
        }
        if (!TryComp(uid, out ThrusterComponent? thruster)) return true;
        if (!TryComp(uid, out TransformComponent? xForm)) return true;
        if (core.Working)
            _thrusterSystem.EnableThruster(uid, thruster, xForm);
        else
            _thrusterSystem.DisableThruster(uid, thruster, xForm);
        return !supplier.Enabled && !receiver.PowerDisabled; // i.e. PowerEnabled
    }

    private void OnNewLink(EntityUid uid, EnergyCoreConsoleComponent component, NewLinkEvent args)
    {
        if (!TryComp<EnergyCoreComponent>(args.Sink, out var analyzer))
            return;

        component.EnergyCoreEntity = args.Sink;
        analyzer.EnergyCoreConsoleEntity = uid;
    }

    private void OnTryOpenUI(EntityUid console, EnergyCoreConsoleComponent component, UserOpenActivatableUIAttemptEvent args)
    {
        if (component.EnergyCoreEntity is not EntityUid entity)
        {
            args.Cancel();
        }
    }
    private void OnPowerToggled(EntityUid uid, EnergyCoreConsoleComponent component, EnergyCoreConsoleIsOnMessage args)
    {
        if (component.EnergyCoreEntity is null || !TryComp(component.EnergyCoreEntity, out EnergyCoreComponent? core)) return;
        TogglePower(component.EnergyCoreEntity.Value);
    }
    private void OnParentChanged(EntityUid uid, EnergyCoreComponent component, ref EntParentChangedMessage args)
    {
        if (TryComp(args.OldParent, out GravityComponent? gravity))
        {
            _gravitySystem.RefreshGravity(args.OldParent.Value, gravity);
        }
    }
}

using Content.Server.PowerCell;
using Content.Shared.Alert;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Robust.Shared.Player;
using Content.Shared._Adventure.Races.IPC;

namespace Content.Server._Adventure.Races.IPC;

/// <summary>
/// Код заимствован из BorgSystem с удалением всего лишнего.  
/// В текущей версии компонент не имеет отличий от BorgSystem.
/// </summary>
public sealed partial class IPCSystem : SharedIPCSystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifier = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<IPCComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<IPCComponent, MindAddedMessage>(OnMindAdded);
        SubscribeLocalEvent<IPCComponent, MindRemovedMessage>(OnMindRemoved);
        SubscribeLocalEvent<IPCComponent, MobStateChangedEvent>(OnMobStateChanged);
        SubscribeLocalEvent<IPCComponent, PowerCellChangedEvent>(OnPowerCellChanged);
        SubscribeLocalEvent<IPCComponent, PowerCellSlotEmptyEvent>(OnPowerCellSlotEmpty);
        SubscribeLocalEvent<IPCComponent, GetCharactedDeadIcEvent>(OnGetDeadIC);
        SubscribeLocalEvent<IPCComponent, ItemToggledEvent>(OnToggled);
    }

    private void OnMapInit(EntityUid uid, IPCComponent component, MapInitEvent args)
    {
        UpdateBatteryAlert((uid, component));
        _movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
    }

    private void OnMindAdded(EntityUid uid, IPCComponent component, MindAddedMessage args)
    {
        IPCActivate(uid, component);
    }

    private void OnMindRemoved(EntityUid uid, IPCComponent component, MindRemovedMessage args)
    {
        IPCDeactivate(uid, component);
    }

    private void OnMobStateChanged(EntityUid uid, IPCComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Alive)
        {
            if (_mind.TryGetMind(uid, out _, out _))
                _powerCell.SetDrawEnabled(uid, true);
        }
        else
        {
            _powerCell.SetDrawEnabled(uid, false);
        }
    }

    private void OnPowerCellChanged(EntityUid uid, IPCComponent component, PowerCellChangedEvent args)
    {
        UpdateBatteryAlert((uid, component));

        // if we aren't drawing and suddenly get enough power to draw again, reeanble.
        if (_powerCell.HasDrawCharge(uid))
        {
            Toggle.TryActivate(uid);
        }
    }

    private void OnPowerCellSlotEmpty(EntityUid uid, IPCComponent component, ref PowerCellSlotEmptyEvent args)
    {
        Toggle.TryDeactivate(uid);
    }

    private void OnGetDeadIC(EntityUid uid, IPCComponent component, ref GetCharactedDeadIcEvent args)
    {
        args.Dead = true;
    }

    private void OnToggled(Entity<IPCComponent> ent, ref ItemToggledEvent args)
    {
        var (uid, comp) = ent;
        // only enable the powerdraw if there is a player in the chassis
        var drawing = _mind.TryGetMind(uid, out _, out _) && _mobState.IsAlive(ent);
        _powerCell.SetDrawEnabled(uid, drawing);

        _movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
    }

    private void UpdateBatteryAlert(Entity<IPCComponent> ent, PowerCellSlotComponent? slotComponent = null)
    {
        if (!_powerCell.TryGetBatteryFromSlot(ent, out var battery, slotComponent))
        {
            _alerts.ClearAlert(ent, ent.Comp.BatteryAlert);
            _alerts.ShowAlert(ent, ent.Comp.NoBatteryAlert);
            return;
        }

        var chargePercent = (short) MathF.Round(battery.CurrentCharge / battery.MaxCharge * 10f);

        // we make sure 0 only shows if they have absolutely no battery.
        // also account for floating point imprecision
        if (chargePercent == 0 && _powerCell.HasDrawCharge(ent, cell: slotComponent))
        {
            chargePercent = 1;
        }

        _alerts.ClearAlert(ent, ent.Comp.NoBatteryAlert);
        _alerts.ShowAlert(ent, ent.Comp.BatteryAlert, chargePercent);
    }

    public void IPCActivate(EntityUid uid, IPCComponent component)
    {
        if (_powerCell.HasDrawCharge(uid))
        {
            Toggle.TryActivate(uid);
            _powerCell.SetDrawEnabled(uid, _mobState.IsAlive(uid));
        }
    }

    public void IPCDeactivate(EntityUid uid, IPCComponent component)
    {
        Toggle.TryDeactivate(uid);
        _powerCell.SetDrawEnabled(uid, false);
    }
}

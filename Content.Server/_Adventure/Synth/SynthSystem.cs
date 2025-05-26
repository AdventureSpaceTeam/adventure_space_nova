using Content.Server.Actions;
using Content.Server.Emp;
using Content.Shared.Stunnable;
using Content.Shared.Damage;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Hands.Systems;
using Content.Server.PowerCell;
using Content.Shared.Alert;
using Content.Shared.Database;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared._Adventure.Synth;
using Content.Shared._Adventure.Synth.Components;
using Content.Shared.Throwing;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared.Ninja.Components;
using Content.Shared.Ninja.Systems;
using Content.Shared.Popups;
using Content.Shared.Actions;


namespace Content.Server._Adventure.Synth;

public sealed partial class SynthSystem : SharedSynthSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly DeviceNetworkSystem _deviceNetwork = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly TriggerSystem _trigger = default!;
    [Dependency] private readonly HandsSystem _hands = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifier = default!;
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedBatteryDrainerSystem _batteryDrainer = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedActionsSystem _action = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SynthComponent, EmpPulseEvent>(OnEmpPulse);
        SubscribeLocalEvent<SynthComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<SynthComponent, PowerCellChangedEvent>(OnPowerCellChanged);
        SubscribeLocalEvent<SynthComponent, PowerCellSlotEmptyEvent>(OnPowerCellSlotEmpty);
        SubscribeLocalEvent<SynthComponent, ItemToggledEvent>(OnToggled);
        SubscribeLocalEvent<SynthComponent, ToggleDrainActionEvent>(OnToggleAction);
        SubscribeLocalEvent<SynthComponent, ComponentShutdown>(OnComponentShutdown);

        InitializeUI();

    }

    private void OnEmpPulse(EntityUid uid, SynthComponent component, EmpPulseEvent ev)
    {
        _damageableSystem.TryChangeDamage(uid, component.EmpDamage, true);
        _stunSystem.TryParalyze(uid, TimeSpan.FromSeconds(component.EmpParalyzeTime), true);
    }

    private void OnMapInit(EntityUid uid, SynthComponent component, MapInitEvent args)
    {
        UpdateBatteryAlert((uid, component));
        _movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
        _action.AddAction(uid, ref component.ActionEntity, component.DrainBatteryAction);
    }

    private void UpdateBatteryAlert(Entity<SynthComponent> ent, PowerCellSlotComponent? slotComponent = null)
    {
        if (!_powerCell.TryGetBatteryFromSlot(ent, out var battery, slotComponent))
        {
            _alerts.ClearAlert(ent, ent.Comp.BatteryAlert);
            _alerts.ShowAlert(ent, ent.Comp.NoBatteryAlert);
            return;
        }

        var chargePercent = (short) MathF.Round(battery.CurrentCharge / battery.MaxCharge * 10f);

        if (chargePercent == 0 && _powerCell.HasDrawCharge(ent, cell: slotComponent))
        {
            chargePercent = 1;
        }

        _alerts.ClearAlert(ent, ent.Comp.NoBatteryAlert);
        _alerts.ShowAlert(ent, ent.Comp.BatteryAlert, chargePercent);
    }

    private void OnPowerCellChanged(EntityUid uid, SynthComponent component, PowerCellChangedEvent args)
    {
        UpdateBatteryAlert((uid, component));

        if (_powerCell.HasDrawCharge(uid))
        {
            Toggle.TryActivate(uid);
        }
        UpdateUI(uid, component);
    }

    private void OnPowerCellSlotEmpty(EntityUid uid, SynthComponent component, ref PowerCellSlotEmptyEvent args)
    {
        Toggle.TryDeactivate(uid);
        UpdateUI(uid, component);
    }

    private void OnToggled(Entity<SynthComponent> ent, ref ItemToggledEvent args) 
    {
        var (uid, comp) = ent;

        var drawing = _mobState.IsAlive(ent);
        _powerCell.SetDrawEnabled(uid, drawing);

        UpdateUI(uid, comp);

        _movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
    }
    private void OnToggleAction(EntityUid uid, SynthComponent component, ToggleDrainActionEvent args)
    {
        if (args.Handled)
            return;

        component.DrainActivated = !component.DrainActivated;
        _action.SetToggled(component.ActionEntity, component.DrainActivated);
        args.Handled = true;

        if (component.DrainActivated && _powerCell.TryGetBatteryFromSlot(uid, out var battery, out var _))
        {
            EnsureComp<BatteryDrainerComponent>(uid);
            _batteryDrainer.SetBattery(uid, battery);
        }
        else
            RemComp<BatteryDrainerComponent>(uid);

        var message = component.DrainActivated ? "ipc-component-ready" : "ipc-component-disabled";
        _popup.PopupEntity(Loc.GetString(message), uid, uid);
    }
    private void OnComponentShutdown(EntityUid uid, SynthComponent component, ComponentShutdown args)
    {
        _action.RemoveAction(uid, component.ActionEntity);
    }
}

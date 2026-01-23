using System.Linq;
using Content.Shared.UserInterface;
using Content.Shared.Database;
using Content.Shared.NameIdentifier;
using Content.Shared.PowerCell.Components;
using Content.Shared.Preferences;
using Content.Shared._Adventure.Synth;
using Content.Shared._Adventure.Synth.Components;

namespace Content.Server._Adventure.Synth;

/// <inheritdoc/>
public sealed partial class SynthSystem
{
    public void InitializeUI()
    {
        SubscribeLocalEvent<SynthComponent, BeforeActivatableUIOpenEvent>(OnBeforeSynthUiOpen);
        SubscribeLocalEvent<SynthComponent, SynthEjectBatteryBuiMessage>(OnEjectBatteryBuiMessage);
    }

    private void OnBeforeSynthUiOpen(EntityUid uid, SynthComponent component, BeforeActivatableUIOpenEvent args)
    {
        UpdateUI(uid, component);
    }

    private void OnEjectBatteryBuiMessage(EntityUid uid, SynthComponent component, SynthEjectBatteryBuiMessage args)
    {
        if (!TryComp<PowerCellSlotComponent>(uid, out var slotComp) ||
            !Container.TryGetContainer(uid, slotComp.CellSlotId, out var container) ||
            !container.ContainedEntities.Any())
        {
            return;
        }

        var ents = Container.EmptyContainer(container);
        _hands.TryPickupAnyHand(args.Actor, ents.First());
    }

    public void UpdateBattery(Entity<SynthComponent> ent)
    {
        UpdateBatteryAlert(ent);
        if (_powerCell.HasDrawCharge(ent.Owner))
        {
            Toggle.TryActivate(ent.Owner);
        }

        UpdateUI(ent, ent);
    }

    private void UpdateBatteryAlert(Entity<SynthComponent> ent, PowerCellSlotComponent? slotComponent = null)
    {
        if (!_powerCell.TryGetBatteryFromSlot((ent.Owner, slotComponent), out var battery))
        {
            _alerts.ClearAlert(ent.Owner, ent.Comp.BatteryAlert);
            _alerts.ShowAlert(ent.Owner, ent.Comp.NoBatteryAlert);
            return;
        }

        var chargePercent = (short)MathF.Round(_battery.GetCharge(battery.Value.AsNullable()) / battery.Value.Comp.MaxCharge * 10f);

        if (chargePercent == 0 && _powerCell.HasDrawCharge((ent.Owner, null, slotComponent)))
        {
            chargePercent = 1;
        }

        _alerts.ClearAlert(ent.Owner, ent.Comp.NoBatteryAlert);
        _alerts.ShowAlert(ent.Owner, ent.Comp.BatteryAlert, chargePercent);
    }

    public void UpdateUI(EntityUid uid, SynthComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        var chargePercent = 0f;
        var hasBattery = false;
        if (_powerCell.TryGetBatteryFromSlot(uid, out var battery))
        {
            hasBattery = true;
            chargePercent = _battery.GetCharge(battery.Value.AsNullable()) / battery.Value.Comp.MaxCharge;
        }

        var state = new SynthBuiState(chargePercent, hasBattery);
        _ui.SetUiState(uid, SynthUiKey.Key, state);
    }

    public void UpdateBattery(float frameTime)
    {
        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<SynthComponent>();
        while (query.MoveNext(out var uid, out var synth))
        {
            if (curTime < synth.NextBatteryUpdate)
                continue;

            UpdateBatteryAlert((uid, synth));
            UpdateBattery((uid, synth));
            synth.NextBatteryUpdate = curTime + TimeSpan.FromSeconds(1);
        }
    }
}

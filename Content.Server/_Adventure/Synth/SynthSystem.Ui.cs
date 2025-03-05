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

    public void UpdateUI(EntityUid uid, SynthComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        var chargePercent = 0f;
        var hasBattery = false;
        if (_powerCell.TryGetBatteryFromSlot(uid, out var battery))
        {
            hasBattery = true;
            chargePercent = battery.CurrentCharge / battery.MaxCharge;
        }

        var state = new SynthBuiState(chargePercent, hasBattery);
        _ui.SetUiState(uid, SynthUiKey.Key, state);
    }
}

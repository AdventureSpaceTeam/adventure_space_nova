using Content.Shared.AdventureSpace.Vampire.Attempt;
using Content.Shared.Inventory;

namespace Content.Server.AdventureSpace.DarkForces.Saint.Items.Cross;

public sealed partial class SaintCrossSystem
{
    private void InitializeVampire()
    {
        SubscribeLocalEvent<SaintCrossComponent, InventoryRelayedEvent<VampireDrinkBloodAttemptEvent>>(
            OnInventoriedSaintedCrossEvent);
        SubscribeLocalEvent<SaintCrossComponent, InventoryRelayedEvent<VampireHypnosisAttemptEvent>>(
            OnVampireHypnosisAttemptEvent);
        SubscribeLocalEvent<SaintCrossComponent, InventoryRelayedEvent<VampireParalizeAttemptEvent>>(
            OnVampireParalyzeAttemptEvent);
        SubscribeLocalEvent<SaintCrossComponent, InventoryRelayedEvent<VampireChiropteanScreechAttemptEvent>>(
            OnScreechAttemptEvent);
    }

    private void OnScreechAttemptEvent(EntityUid uid,
        SaintCrossComponent component,
        InventoryRelayedEvent<VampireChiropteanScreechAttemptEvent> args)
    {
        args.Args.Cancel();
    }

    private void OnVampireParalyzeAttemptEvent(EntityUid uid,
        SaintCrossComponent component,
        InventoryRelayedEvent<VampireParalizeAttemptEvent> args)
    {
        args.Args.Cancel();
    }

    private void OnVampireHypnosisAttemptEvent(EntityUid uid,
        SaintCrossComponent component,
        InventoryRelayedEvent<VampireHypnosisAttemptEvent> args)
    {
        args.Args.Cancel();
    }

    private void OnInventoriedSaintedCrossEvent(EntityUid uid,
        SaintCrossComponent component,
        InventoryRelayedEvent<VampireDrinkBloodAttemptEvent> args)
    {
        args.Args.Cancel();
    }
}

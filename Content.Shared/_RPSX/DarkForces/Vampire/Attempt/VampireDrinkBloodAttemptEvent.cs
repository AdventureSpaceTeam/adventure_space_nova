using Content.Shared.Inventory;

namespace Content.Shared.RPSX.Vampire.Attempt;

public sealed class VampireDrinkBloodAttemptEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
    public SlotFlags TargetSlots => SlotFlags.NECK;
}

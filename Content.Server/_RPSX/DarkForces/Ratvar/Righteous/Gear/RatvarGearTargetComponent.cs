using Content.Shared.Containers.ItemSlots;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Gear;

[RegisterComponent]
public sealed partial class RatvarGearTargetComponent : Component
{
    public readonly string GearSlotId = "GearSlot";

    [DataField("gearSlot", required: true)]
    public ItemSlot GearSlot = new();
}

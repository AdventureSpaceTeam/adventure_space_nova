namespace Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities.Slab;

[RegisterComponent]
public sealed partial class RatvarHidingItemComponent : Component
{
    [DataField]
    public EntityUid? OriginalItem;
}

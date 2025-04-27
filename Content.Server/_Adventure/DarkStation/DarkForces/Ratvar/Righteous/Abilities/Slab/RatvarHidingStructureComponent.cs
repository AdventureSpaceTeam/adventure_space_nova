namespace Content.Server._Adventure.DarkForces.Ratvar.Righteous.Abilities.Slab;

[RegisterComponent]
public sealed partial class RatvarHidingStructureComponent : Component
{
    [DataField]
    public EntityUid? HidingSlab;

    [DataField]
    public EntityUid? OriginalStructure;
}

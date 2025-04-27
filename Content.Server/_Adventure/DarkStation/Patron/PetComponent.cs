namespace Content.Server._Adventure.Patron;

[RegisterComponent]
public sealed partial class PetComponent : Component
{
    [DataField]
    public EntityUid? PetOwner;
}

namespace Content.Server.AdventureSpace.Patron;

[RegisterComponent]
public sealed partial class PetComponent : Component
{
    [DataField]
    public EntityUid? PetOwner;
}

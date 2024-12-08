namespace Content.Server.AdventurePrivate._Alteros.Patron;

[RegisterComponent]
public sealed partial class PetComponent : Component
{
    [DataField]
    public EntityUid? PetOwner;
}

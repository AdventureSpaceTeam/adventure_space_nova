namespace Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Bionic;

[RegisterComponent]
public sealed partial class ArmDrillComponent : Component, IBionicPart
{
    [DataField]
    public List<EntityUid> EntitesUids { get; set; } = new();
}

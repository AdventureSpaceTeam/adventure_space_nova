namespace Content.Server.AdventureSpace.Medical.Surgery.Bionic;

[RegisterComponent]
public sealed partial class ArmDrillComponent : Component, IBionicPart
{
    [DataField]
    public List<EntityUid> EntitesUids { get; set; } = new();
}

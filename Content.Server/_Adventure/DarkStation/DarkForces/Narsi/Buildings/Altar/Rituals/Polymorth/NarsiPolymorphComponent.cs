namespace Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;

[RegisterComponent]
public sealed partial class NarsiPolymorphComponent : Component
{
    [DataField]
    public EntityUid AltarEntityUid;

    [DataField]
    public bool ReturnToAltar;
}

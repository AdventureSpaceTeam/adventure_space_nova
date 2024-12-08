namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Runes.Components;

[RegisterComponent]
public sealed partial class NarsiTeleportRuneComponent : Component
{
    [DataField("tag")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string Tag = "";
}

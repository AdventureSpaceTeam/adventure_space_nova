using Robust.Shared.GameStates;

namespace Content.Shared._Adventure.DarkForces.Narsi.Roles;

[RegisterComponent, NetworkedComponent]
public sealed partial class NarsiCultistComponent : Component
{
    [DataField]
    public Dictionary<string, EntityUid?> Abilities = new();
}

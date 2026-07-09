using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Vampire.Rule.Objectives.Enthrall;

[RegisterComponent]
public sealed partial class VampireEnthrallObjectiveComponent : Component
{
    [DataField]
    public float TrallCount = 2f;
}

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Vampire.Rule.Objectives.Blood;

[RegisterComponent]
public sealed partial class VampireBloodObjectiveComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public float RequiredBloodCount = 4000f;
}

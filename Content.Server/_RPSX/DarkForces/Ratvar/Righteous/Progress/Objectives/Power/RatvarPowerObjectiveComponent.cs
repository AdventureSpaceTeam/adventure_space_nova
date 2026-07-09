using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Progress.Objectives.Power;

[RegisterComponent]
public sealed partial class RatvarPowerObjectiveComponent : Component
{
    [DataField]
    public int RequiredCount = 20000;
}

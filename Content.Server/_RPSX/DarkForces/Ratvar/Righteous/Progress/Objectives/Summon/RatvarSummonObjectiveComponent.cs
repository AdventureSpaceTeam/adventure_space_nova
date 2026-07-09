using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Progress.Objectives.Summon;

[RegisterComponent]
public sealed partial class RatvarSummonObjectiveComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public bool IsCompleted;

    [DataField]
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? Target;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan UpdateCoordinatesTime;

    [DataField]
    public TimeSpan UpdateCoordinatesPeriod = TimeSpan.FromSeconds(10);
}

using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Progress;

[RegisterComponent]
public sealed partial class RatvarProgressComponent : Component
{
    [DataField]
    public int CurrentPower;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan NextObjectivesCheckTick;

    [DataField]
    public TimeSpan ObjectivesCheckPeriod = TimeSpan.FromSeconds(30);

    [DataField]
    public EntityUid RatvarBeaconsObjective = EntityUid.Invalid;

    [DataField]
    public EntityUid RatvarConvertObjective = EntityUid.Invalid;

    [DataField]
    public EntityUid RatvarPowerObjective = EntityUid.Invalid;

    [DataField]
    public EntityUid RatvarSummonObjective = EntityUid.Invalid;
}

using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.AdventureSpace.GameRules.SCP.Station.Components;

[RegisterComponent]
public sealed partial class SCPStationGoalComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan NextTaskDelay;

    [DataField]
    public TimeSpan NextTaskThreshold = TimeSpan.FromMinutes(5);
}

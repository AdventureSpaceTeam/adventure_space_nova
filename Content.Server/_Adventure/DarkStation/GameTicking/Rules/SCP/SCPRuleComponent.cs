using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Content.Server._Adventure.GameTicking.Rules.SCP;

[RegisterComponent]
public sealed partial class SCPRuleComponent : Component
{
    [DataField(customTypeSerializer: typeof(ResPathSerializer))]
    public ResPath SCPShuttleMap = new("/Maps/DarkStationShuttles/SCP/SCPShuttle.yml");

    [DataField(customTypeSerializer: typeof(ResPathSerializer))]
    public ResPath SCPStationMap = new("/Maps/DarkStationMaps/SCP/SCPStation.yml");
}

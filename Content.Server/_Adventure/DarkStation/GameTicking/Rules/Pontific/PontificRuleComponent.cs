using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.AdventureSpace.GameTicking.Rules.Pontific;

[RegisterComponent] [Access(typeof(PontificRuleSystem))]
public sealed partial class PontificRuleComponent : Component
{
    [DataField]
    public float DeathShuttleCallPercentage = 0.5f;

    [DataField]
    public TimeSpan EndCheckDelay = TimeSpan.FromSeconds(30);

    [DataField]
    public bool IsPontificDead;

    [DataField]
    public int MinPlayers = 30;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextRoundEndCheck;

    [DataField]
    public ProtoId<AntagPrototype> PontificPrefId = "PontificAntag";

    [DataField]
    public bool ShuttleCalled;
}

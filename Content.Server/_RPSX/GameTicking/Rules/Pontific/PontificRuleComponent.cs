using System;
using Content.Shared.Roles;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.RPSX.GameTicking.Rules.Pontific;

[RegisterComponent, Access(typeof(PontificRuleSystem))]
public sealed partial class PontificRuleComponent : Component
{
    [DataField]
    public ProtoId<AntagPrototype> PontificPrefId = "PontificAntag";

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextRoundEndCheck;

    [DataField]
    public TimeSpan EndCheckDelay = TimeSpan.FromSeconds(30);

    [DataField]
    public int MinPlayers = 30;

    [DataField]
    public float DeathShuttleCallPercentage = 0.5f;

    [DataField]
    public bool ShuttleCalled;

    [DataField]
    public bool IsPontificDead;
}

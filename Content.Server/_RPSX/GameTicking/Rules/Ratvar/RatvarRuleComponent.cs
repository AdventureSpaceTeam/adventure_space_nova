using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.RPSX.GameTicking.Rules.Ratvar;

[RegisterComponent]
[Access(typeof(RatvarRuleSystem))]
public sealed partial class RatvarRuleComponent : Component
{
    [DataField]
    public WinState WinState = WinState.Idle;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan ForceRoundEnd;
}

public enum WinState
{
    Idle = 0,
    Summoning = 1,
    RighteousWon = 2
}

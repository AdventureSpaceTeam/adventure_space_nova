using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.AdventurePrivate._Alteros.GameTicking.Rules.Ratvar;

[RegisterComponent]
[Access(typeof(RatvarRuleSystem))]
public sealed partial class RatvarRuleComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan ForceRoundEnd;

    [DataField]
    public WinState WinState = WinState.Idle;
}

public enum WinState
{
    Idle = 0,
    Summoning = 1,
    RighteousWon = 2,
}

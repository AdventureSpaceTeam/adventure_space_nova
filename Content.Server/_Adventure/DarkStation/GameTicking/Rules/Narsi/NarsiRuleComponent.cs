using Robust.Shared.Audio;

namespace Content.Server._Adventure.GameTicking.Rules.Narsi;

[RegisterComponent] [Access(typeof(NarsiRuleSystem))]
public sealed partial class NarsiRuleComponent : Component
{
    [DataField]
    public SoundSpecifier NarsiExileSound = new SoundPathSpecifier("/Audio/DarkStation/Narsi/narsi_destroy.ogg");

    public TimeSpan NarsiRepeatSoundAt = TimeSpan.Zero;

    [DataField]
    public SoundSpecifier NarsiSummonSound = new SoundPathSpecifier("/Audio/DarkStation/Narsi/narsi_summon.ogg");

    public TimeSpan RoundEndAt = TimeSpan.Zero;

    [DataField]
    public EntityUid RuneSource = EntityUid.Invalid;

    [DataField]
    public WinState WinStateStatus = WinState.Idle;
}

public enum WinState
{
    Idle,
    NarsiSummoning,
    NarsiLastStand,
    CultistWon,
}

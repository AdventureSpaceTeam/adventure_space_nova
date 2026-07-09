using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.GameTicking.Rules.Narsi;

[RegisterComponent] [Access(typeof(NarsiRuleSystem))]
public sealed partial class NarsiRuleComponent : Component
{
    [DataField]
    public WinState WinStateStatus = WinState.Idle;

    [DataField]
    public EntityUid RuneSource = EntityUid.Invalid;

    [DataField]
    public SoundSpecifier NarsiExileSound = new SoundPathSpecifier("/Audio/DarkStation/Narsi/narsi_destroy.ogg");

    [DataField]
    public SoundSpecifier NarsiSummonSound = new SoundPathSpecifier("/Audio/DarkStation/Narsi/narsi_summon.ogg");

    public TimeSpan NarsiRepeatSoundAt = TimeSpan.Zero;
    public TimeSpan RoundEndAt = TimeSpan.Zero;
}

public enum WinState
{
    Idle,
    NarsiSummoning,
    NarsiLastStand,
    CultistWon
}

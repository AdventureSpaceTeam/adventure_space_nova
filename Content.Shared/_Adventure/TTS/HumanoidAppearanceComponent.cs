using Content.Shared._Adventure.TTS;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid;

public sealed partial class HumanoidAppearanceComponent : Component
{
    /// <summary>
    ///     Current voice. Used for correct cloning.
    /// </summary>
    [DataField]
    public ProtoId<TTSVoicePrototype> Voice { get; set; } = TTSConfig.DefaultVoice;
}

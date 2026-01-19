using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;

namespace Content.Shared._Adventure.TTS;

/// <summary>
/// Prototype represent available TTS voices
/// </summary>
[Prototype("ttsVoice")]
// ReSharper disable once InconsistentNaming
public sealed partial class TTSVoicePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("name")]
    public string Name { get; private set; } = string.Empty;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("speaker", required: true)]
    public string Speaker { get; private set; } = string.Empty;

    [DataField("sponsorOnly")]
    public bool SponsorOnly { get; private set; } = false;
}

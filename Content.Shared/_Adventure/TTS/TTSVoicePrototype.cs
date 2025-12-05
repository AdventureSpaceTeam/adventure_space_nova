using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;

namespace Content.Shared._Adventure.TTS;

/// <summary>
/// Prototype represent available TTS voices
/// </summary>
[Prototype("ttsVoice")]
// ReSharper disable once InconsistentNaming
public sealed class TTSVoicePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("name")]
    public string Name { get; } = string.Empty;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("speaker", required: true)]
    public string Speaker { get; } = string.Empty;

    [DataField("sponsorOnly")]
    public bool SponsorOnly { get; } = false;

    [DataField("gender")]
    public string Gender { get; } = "None";
}

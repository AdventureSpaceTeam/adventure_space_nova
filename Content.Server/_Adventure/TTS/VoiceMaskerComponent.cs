using Content.Shared.Speech;
using Robust.Shared.Prototypes;

// c4llv07e: See voicemask component.
namespace Content.Server.VoiceMask;

public sealed partial class VoiceMaskerComponent : Component
{
    [DataField]
    public string? LastSetVoice;
}

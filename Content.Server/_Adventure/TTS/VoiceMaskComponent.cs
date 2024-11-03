using Content.Shared.Humanoid;
using Content.Shared._Adventure.TTS;

/*
  Yep, that's right, this file isn't located in the
  Content.Server/VoiceMask, but we still use this namespace just to
  add ours fields.
*/
namespace Content.Server.VoiceMask;

public sealed partial class VoiceMaskComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public string VoiceId = TTSConfig.DefaultVoice;
}

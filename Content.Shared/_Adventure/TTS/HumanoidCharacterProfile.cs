using Content.Shared._Adventure.TTS;
/*
    Because we are appending components to the existing class,
    we need to use its namespace.
*/
namespace Content.Shared.Preferences;
public sealed partial class HumanoidCharacterProfile : ICharacterProfile
{
    [DataField]
    public string Voice { get; set; } = TTSConfig.DefaultVoice;
}

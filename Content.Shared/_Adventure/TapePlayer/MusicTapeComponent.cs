using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared._Adventure.TapePlayer;

[RegisterComponent, NetworkedComponent]
public sealed partial class MusicTapeComponent : Component
{
    [DataField("sound", customTypeSerializer: typeof(SoundSpecifierTypeSerializer))]
    public SoundSpecifier? Sound;
}

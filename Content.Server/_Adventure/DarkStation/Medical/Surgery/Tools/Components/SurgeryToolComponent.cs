using Robust.Shared.Audio;

namespace Content.Server._Adventure.Medical.Surgery.Tools.Components;

[RegisterComponent]
public sealed partial class SurgeryToolComponent : Component
{
    [DataField]
    public float ApplyTime = 3.0f;

    [DataField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Effects/lightburn.ogg");
}

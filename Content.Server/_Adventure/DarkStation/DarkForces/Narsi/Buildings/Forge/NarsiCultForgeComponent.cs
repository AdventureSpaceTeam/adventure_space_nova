using Content.Shared._Adventure.DarkForces.Narsi.Buildings;
using Content.Shared.Materials;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Adventure.DarkForces.Narsi.Buildings.Forge;

[RegisterComponent]
public sealed partial class NarsiCultForgeComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan DelayThreshold = TimeSpan.FromSeconds(45);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan DelayTick;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan DoAfterDelay = TimeSpan.FromSeconds(7);

    [DataField]
    public SoundSpecifier ForgeSound = new SoundPathSpecifier("/Audio/DarkStation/DarkForces/Cult/forge_work.ogg");

    [DataField]
    public AudioParams ForgeSoundParams = AudioParams.Default.WithVariation(0.05f).WithVolume(0.5f);

    [DataField]
    public ProtoId<MaterialPrototype> Plasteel = "Plasteel";

    [DataField]
    public ProtoId<MaterialPrototype> RunicPlasteel = "RunicPlasteel";

    [DataField]
    public NarsiForgeState State = NarsiForgeState.Idle;

    [DataField]
    public ProtoId<MaterialPrototype> Steel = "Steel";
}

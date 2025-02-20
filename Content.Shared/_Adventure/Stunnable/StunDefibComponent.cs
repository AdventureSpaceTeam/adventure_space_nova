using Content.Shared._Adventure.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Server.Stunnable.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(SharedStunDefibSystem))]
public sealed partial class StunDefibComponent : Component
{
    [DataField("energyPerUse"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public float EnergyPerUse = 120;

    [DataField("sparksSound")]
    public SoundSpecifier SparksSound = new SoundCollectionSpecifier("sparks");
}

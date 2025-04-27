using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Medical.Surgery.Bionic;

[RegisterComponent]
public sealed partial class BionicBodyPartComponent : Component
{
    [DataField("components")]
    [AlwaysPushInheritance]
    public ComponentRegistry Components { get; private set; } = new();
}

using Content.Server._Adventure.Medical.Disease.Data;

namespace Content.Server._Adventure.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseProtectionClothingComponent : Component
{
    [DataField]
    public Dictionary<DiseaseSpreading, float> DiseaseProtection = [];
}

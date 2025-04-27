using Content.Server.AdventureSpace.Medical.Disease.Data;

namespace Content.Server.AdventureSpace.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseProtectionClothingComponent : Component
{
    [DataField]
    public Dictionary<DiseaseSpreading, float> DiseaseProtection = [];
}

using Content.Server._Adventure.Medical.Disease.Data;

namespace Content.Server._Adventure.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseTargetComponent : Component
{
    [DataField]
    public Dictionary<string, DiseaseData> Diseases { get; set; } = new();
}

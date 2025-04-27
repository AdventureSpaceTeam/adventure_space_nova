using Content.Server._Adventure.Medical.Disease.Data;

namespace Content.Server._Adventure.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseasesControllerComponent : Component
{
    [DataField]
    public Dictionary<string, DiseaseCureData> DiseasesCures = [];
}

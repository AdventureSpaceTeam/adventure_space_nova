using Content.Server.AdventureSpace.Medical.Disease.Data;

namespace Content.Server.AdventureSpace.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseTargetComponent : Component
{
    [DataField]
    public Dictionary<string, DiseaseData> Diseases { get; set; } = new();
}

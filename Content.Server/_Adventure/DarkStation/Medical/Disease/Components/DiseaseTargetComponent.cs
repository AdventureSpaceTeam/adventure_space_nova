using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Data;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseTargetComponent : Component
{
    [DataField]
    public Dictionary<string, DiseaseData> Diseases { get; set; } = new();
}

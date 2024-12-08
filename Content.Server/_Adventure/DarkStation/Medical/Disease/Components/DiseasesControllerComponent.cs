using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Data;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseasesControllerComponent : Component
{
    [DataField]
    public Dictionary<string, DiseaseCureData> DiseasesCures = [];
}

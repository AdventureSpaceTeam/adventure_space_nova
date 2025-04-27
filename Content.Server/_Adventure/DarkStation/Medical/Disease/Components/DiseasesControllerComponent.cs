using Content.Server.AdventureSpace.Medical.Disease.Data;

namespace Content.Server.AdventureSpace.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseasesControllerComponent : Component
{
    [DataField]
    public Dictionary<string, DiseaseCureData> DiseasesCures = [];
}

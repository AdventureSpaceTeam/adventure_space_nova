using Content.Server.AdventureSpace.Medical.Disease.Effects;

namespace Content.Server.AdventureSpace.Medical.Disease.Data;

[DataDefinition]
public sealed partial class DiseaseSymptom
{
    [DataField(required: true)]
    public string Name { get; set; } = default!;

    [DataField(required: true)]
    public List<DiseaseEffect> Effects { get; set; } = default!;
}

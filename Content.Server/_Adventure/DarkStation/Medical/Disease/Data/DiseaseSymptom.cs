using Content.Server._Adventure.Medical.Disease.Effects;

namespace Content.Server._Adventure.Medical.Disease.Data;

[DataDefinition]
public sealed partial class DiseaseSymptom
{
    [DataField(required: true)]
    public string Name { get; set; } = default!;

    [DataField(required: true)]
    public List<DiseaseEffect> Effects { get; set; } = default!;
}

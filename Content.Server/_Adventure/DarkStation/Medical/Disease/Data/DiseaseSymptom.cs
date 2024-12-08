using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Effects;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Disease.Data;

[DataDefinition]
public sealed partial class DiseaseSymptom
{
    [DataField(required: true)]
    public string Name { get; set; } = default!;

    [DataField(required: true)]
    public List<DiseaseEffect> Effects { get; set; } = default!;
}

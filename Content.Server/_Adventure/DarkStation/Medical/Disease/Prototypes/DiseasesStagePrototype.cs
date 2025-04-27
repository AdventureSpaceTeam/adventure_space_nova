using Content.Server._Adventure.Medical.Disease.Data;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server._Adventure.Medical.Disease.Prototypes;

[Prototype("diseaseStage")]
public sealed class DiseasesStagePrototype : IPrototype
{
    [DataField]
    public Dictionary<ProtoId<ReagentPrototype>, DiseaseCure>? Cure;

    [DataField]
    public ProtoId<DiseaseSymptomPrototype> Symptom { get; set; }

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan SymptomThreshold { get; set; } = TimeSpan.FromMinutes(1);

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan CureIgnoreThreshold { get; set; } = TimeSpan.FromMinutes(1);

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan MutationThreshold { get; set; } = TimeSpan.FromMinutes(2);

    [IdDataField]
    public string ID { get; set; } = default!;

    public DiseaseStage ToDataStage(IPrototypeManager prototypeManager)
    {
        var symptomPrototype = prototypeManager.Index(Symptom);
        var symptomData = new DiseaseSymptom
        {
            Name = symptomPrototype.Name,
            Effects = symptomPrototype.Effects,
        };

        return new DiseaseStage
        {
            Symptom = symptomData,
            SymptomThreshold = SymptomThreshold,
            CureIgnoreThreshold = CureIgnoreThreshold,
            MutationThreshold = MutationThreshold,
            Cure = Cure,
        };
    }
}

using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.AdventureSpace.Medical.Disease.Data;

[Serializable]
[DataDefinition]
public sealed partial class DiseaseStage
{
    [DataField]
    public Dictionary<ProtoId<ReagentPrototype>, DiseaseCure>? Cure;

    [DataField]
    public bool Active { get; set; }

    [DataField]
    public bool Cured { get; set; }

    [DataField]
    public DiseaseSymptom Symptom { get; set; }

    /**
     * Symptom Effect Threshold
     */
    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan SymptomTime { get; set; } = TimeSpan.Zero;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan SymptomThreshold { get; set; }

    /**
     * Cure Stage
     * Каждая болезнь может иметь свои лекарства от симптомов
     * При простуде, попить чайку, чтобы избавиться от кашля/чихания - ок
     * А вот при короне хер ты от них избавишься чайком
     * Поэтому это и прописывается в "стадиях" болезней, которые по сути являются симптомом
     */
    [DataField]
    public TimeSpan CureIgnoreTime { get; set; } = TimeSpan.Zero;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan CureIgnoreThreshold { get; set; }

    /**
     * Mutation Threshold
     */
    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan MutationTime { get; set; } = TimeSpan.Zero;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan MutationThreshold { get; set; }
}

[DataDefinition]
public sealed partial class DiseaseCure
{
    [DataField]
    public float SymptomThresholdIncrease { get; set; } = 3;

    [DataField]
    public bool PausedMutationOnCure { get; set; }

    [DataField]
    public int TimesToCurePermanent { get; set; }

    [DataField]
    public int CurrentCureTimes { get; set; }
}

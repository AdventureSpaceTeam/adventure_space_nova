using Content.Server._Adventure.Medical.Disease.Components;
using Content.Server._Adventure.Medical.Disease.Data;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Medical.Disease.Prototypes;

[Prototype("disease")]
public sealed class DiseasePrototype : IPrototype
{
    [DataField]
    public ProtoId<DiseaseCurePrototype>? Cure;

    [DataField]
    public TimeSpan ImmunityIncreaseThreshold;

    [DataField]
    public ImmunityIncreaseType ImmunityIncreaseType = ImmunityIncreaseType.None;

    [DataField]
    public string Name { get; set; } = default!;

    [DataField(required: true)]
    public List<ProtoId<DiseasesStagePrototype>> Stages { get; set; } = default!;

    [DataField]
    public List<DiseaseSpreading>? Spreads { get; set; }

    [IdDataField]
    public string ID { get; set; } = default!;

    public DiseaseData ToDiseaseData(IPrototypeManager prototypeManager)
    {
        return new DiseaseData
        {
            ImmunityIncreaseThreshold = ImmunityIncreaseThreshold,
            ImmunityIncreaseType = ImmunityIncreaseType,
            Cure = CopyCure(prototypeManager),
            Stages = CopyStages(prototypeManager),
            Spreads = Spreads,
        };
    }

    private List<DiseaseStage> CopyStages(IPrototypeManager prototypeManager)
    {
        var stageCopies = new List<DiseaseStage>();
        foreach (var stage in Stages)
        {
            if (!prototypeManager.TryIndex(stage, out var stagePrototype))
                continue;

            stageCopies.Add(stagePrototype.ToDataStage(prototypeManager));
        }

        return stageCopies;
    }

    private DiseaseCureData? CopyCure(IPrototypeManager prototypeManager)
    {
        return !prototypeManager.TryIndex(Cure, out var cureData) ? null : cureData.CopyCure();
    }
}

using Content.Shared._Adventure.Medical.Diseases.Effects;
using Robust.Server.GameObjects;
using Robust.Shared.Random;

namespace Content.Server._Adventure.Medical.Disease.Effects;

[DataDefinition]
public sealed partial class DiseaseVisualEffect : DiseaseEffect
{
    [DataField]
    public int CurrentStageIndex;

    [DataField]
    public bool RandomizeStages = true;

    [DataField(required: true)]
    public string Sprite;

    [DataField(required: true)]
    public List<DiseaseVisualStage> Stages;

    public override void OnStart(IEntityManager entityManager, EntityUid target)
    {
        base.OnStart(entityManager, target);
        if (RandomizeStages)
            IoCManager.Resolve<IRobustRandom>().Shuffle(Stages);
    }

    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);

        if (Stages.Count == CurrentStageIndex)
            return;

        var stage = Stages[CurrentStageIndex];
        var appearance = entityManager.System<AppearanceSystem>();
        appearance.SetData(target, stage.Type, new DiseaseVisualsData(Sprite, stage.State));

        CurrentStageIndex++;
    }

    public override void Dispose(IEntityManager entityManager, EntityUid target)
    {
        var appearance = entityManager.System<AppearanceSystem>();

        foreach (var stage in Stages)
        {
            appearance.SetData(target, stage.Type, DiseaseClearKey.Clear);
        }
    }

    [DataDefinition]
    public sealed partial class DiseaseVisualStage
    {
        [DataField(required: true)]
        public string State;

        [DataField(required: true)]
        public DiseaseVisualLayers Type;
    }
}

using System.Linq;
using Content.Server._Adventure.Medical.Disease.Components;
using Content.Server._Adventure.Medical.Disease.Data;

namespace Content.Server._Adventure.Medical.Disease.Systems;

public sealed partial class DiseaseSystem
{
    private void TryApplyReagentToSymptom(Entity<DiseaseTargetComponent> target,
        string key,
        DiseaseData data,
        string reagentId)
    {
        foreach (var stage in data.Stages)
        {
            if (stage.Cure == null || !stage.Cure.TryGetValue(reagentId, out var cure))
                continue;

            HandleIncreaseSymptomThreshold(target, key, data, stage, cure);
        }
    }

    private bool HandleIncreaseSymptomThreshold(
        Entity<DiseaseTargetComponent> target,
        string key,
        DiseaseData data,
        DiseaseStage stage,
        DiseaseCure cure
    )
    {
        var curTime = _gameTiming.CurTime;
        if (stage.CureIgnoreTime > curTime)
            return false;

        stage.SymptomTime += stage.SymptomThreshold * cure.SymptomThresholdIncrease;
        stage.CureIgnoreTime = curTime + stage.CureIgnoreThreshold;

        if (CanPauseMutation(stage, cure))
            stage.MutationTime += stage.MutationThreshold * cure.SymptomThresholdIncrease;

        if (CanCurePermanently(stage, cure))
        {
            if (CanCureDisease(target, data))
                RemoveDisease(target, key);

            stage.Cured = true;
        }

        cure.CurrentCureTimes += 1;
        return true;
    }

    private bool CanPauseMutation(DiseaseStage stage, DiseaseCure cure)
    {
        return stage.MutationTime != TimeSpan.Zero && cure.PausedMutationOnCure;
    }

    private bool CanCureDisease(Entity<DiseaseTargetComponent> target, DiseaseData data)
    {
        return data.Stages.All(stage => stage.Cured);
    }

    private bool CanCurePermanently(DiseaseStage stage, DiseaseCure cure)
    {
        return cure.TimesToCurePermanent > 0 && cure.CurrentCureTimes >= cure.TimesToCurePermanent;
    }
}

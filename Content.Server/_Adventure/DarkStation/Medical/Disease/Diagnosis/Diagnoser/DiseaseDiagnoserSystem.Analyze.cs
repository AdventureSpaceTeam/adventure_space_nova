using System.Text;
using Content.Server._Adventure.Medical.Disease.Diagnosis.Diagnoser.Components;
using Content.Server._Adventure.Medical.Disease.Diagnosis.Swab;
using Content.Server._Adventure.Medical.Disease.Prototypes;
using Content.Shared.Paper;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Medical.Disease.Diagnosis.Diagnoser;

public sealed partial class DiseaseDiagnoserSystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string DiagnosisReportPaper = "DiagnosisReportPaper";

    private void PrintDiseases(Entity<DiseaseDiagnoserComponent> diagnoser)
    {
        if (!_slots.TryGetSlot(diagnoser, diagnoser.Comp.SwabSlotId, out var slot))
            return;

        if (!TryComp<DiseaseSwabComponent>(slot.Item, out var swabComp))
            return;

        var builder = new StringBuilder();

        BuildHeader(builder);

        if (swabComp.DiseasesPrototypesIds.Count == 0)
        {
            BuildNoDiseases(builder);
            SpawnPaperAnalyzeResult(builder, diagnoser, swabComp.DiseasesPrototypesIds);
            return;
        }

        foreach (var diseaseId in swabComp.DiseasesPrototypesIds)
        {
            if (!_prototypeManager.TryIndex<DiseasePrototype>(diseaseId, out var diseasePrototype))
                continue;

            BuildDiseaseHeader(builder, diseasePrototype);

            foreach (var stage in diseasePrototype.Stages)
            {
                BuildStage(builder, stage);
            }

            BuildSpreads(builder, diseasePrototype);
            BuildDiseaseCures(builder, diseasePrototype);
        }

        SpawnPaperAnalyzeResult(builder, diagnoser, swabComp.DiseasesPrototypesIds);
    }

    private void BuildHeader(StringBuilder builder)
    {
        var header = Loc.GetString("diagnoser-analyze-result-header");
        builder.AppendLine(header);
    }

    private void BuildNoDiseases(StringBuilder builder)
    {
        var header = Loc.GetString("diagnoser-analyze-result-no-diseases-header");
        builder.AppendLine(header);
    }

    private void BuildDiseaseHeader(StringBuilder builder, DiseasePrototype diseasePrototype)
    {
        var header = Loc.GetString(
            "diagnoser-analyze-result-disease-header",
            ("DiseaseName", diseasePrototype.Name),
            ("DiseaseSymptomsCount", diseasePrototype.Stages.Count)
        );

        builder.AppendLine(header);
    }

    private void BuildStage(StringBuilder builder, ProtoId<DiseasesStagePrototype> stageId)
    {
        if (!_prototypeManager.TryIndex(stageId, out var stage))
            return;

        if (!_prototypeManager.TryIndex(stage.Symptom, out var symptomPrototype))
            return;

        if (stage.Cure is not { } cure)
        {
            var noCureSymptom = Loc.GetString(
                "diagnoser-analyze-result-symptom-no-cures",
                ("DiseaseSymptom", symptomPrototype.Name)
            );
            builder.AppendLine(noCureSymptom);

            return;
        }

        var symptom = Loc.GetString(
            "diagnoser-analyze-result-symptom",
            ("DiseaseSymptom", symptomPrototype.Name)
        );
        builder.AppendLine(symptom);

        BuildStageCures(builder, stage);
    }

    private void BuildStageCures(StringBuilder builder, DiseasesStagePrototype stage)
    {
        foreach (var (id, cureData) in stage.Cure!)
        {
            if (!_prototypeManager.TryIndex(id, out var cureReagentPrototype))
                continue;

            var increaseCure = Loc.GetString(
                "diagnoser-analyze-result-symptom-cure-increase",
                ("DiseaseSymptomCure", cureReagentPrototype.LocalizedName),
                ("CureStoppedMutation", cureData.PausedMutationOnCure ? "Да" : "Нет"),
                ("CureCanHealSymptom", cureData.TimesToCurePermanent > 0 ? "Да" : "Нет"),
                ("NeedSomeTimes", cureData.TimesToCurePermanent > 0 ? "Да" : "Нет")
            );

            builder.AppendLine(increaseCure);
        }
    }

    private void BuildSpreads(StringBuilder builder, DiseasePrototype prototype)
    {
        var header = Loc.GetString("diagnoser-analyze-result-spreads-header");
        builder.AppendLine(header);

        if (prototype.Spreads is not { } spreads)
        {
            builder.AppendLine("diagnoser-analyze-result-spreads-none");
            return;
        }

        foreach (var spreading in spreads)
        {
            var text = $"diagnoser-analyze-result-spreads-{spreading.ToString().ToLower()}";
            builder.AppendLine(Loc.GetString(text));
        }
    }

    private void BuildDiseaseCures(StringBuilder builder, DiseasePrototype prototype)
    {
        var header = Loc.GetString("diagnoser-analyze-result-disease-cure-header");
        builder.AppendLine(header);

        if (!_disease.CanCureDisease(prototype.ID))
        {
            var noCures = Loc.GetString("diagnoser-analyze-result-disease-no-cure");
            builder.AppendLine(noCures);

            return;
        }

        var cure = Loc.GetString("diagnoser-analyze-result-disease-cure");
        builder.AppendLine(cure);
    }

    private void SpawnPaperAnalyzeResult(StringBuilder builder,
        EntityUid diagnoser,
        IEnumerable<string> diseasesPrototypesIDs)
    {
        var paper = Spawn(DiagnosisReportPaper, Transform(diagnoser).Coordinates);

        if (!TryComp<PaperComponent>(paper, out var paperComponent))
            return;

        paperComponent.Content = builder.ToString();

        var resultComp = EnsureComp<DiseaseDiagnoserPaperComponent>(paper);
        resultComp.DiseasesPrototypes.AddRange(diseasesPrototypesIDs);
    }
}

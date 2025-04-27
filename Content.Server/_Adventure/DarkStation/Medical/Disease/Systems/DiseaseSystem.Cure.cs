using System.Linq;
using Content.Server.AdventureSpace.Medical.Disease.Components;
using Content.Server.AdventureSpace.Medical.Disease.Data;
using Content.Server.AdventureSpace.Medical.Disease.Prototypes;
using Content.Server.Medical.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.GameTicking;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.AdventureSpace.Medical.Disease.Systems;

public sealed partial class DiseaseSystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string DiseasesController = "DiseasesController";

    [ValidatePrototypeId<DiseaseBlacklistPrototype>]
    private const string DefaultAnalyzeBlackList = "DefaultAnalyzeBlackList";

    [Dependency]
    private readonly IRobustRandom _random = default!;

    private void InitializeCure()
    {
        SubscribeLocalEvent<DiseaseTargetComponent, ReagentEffectApplyEvent>(OnReagentEffectApply);
        SubscribeLocalEvent<RoundStartedEvent>(OnRoundStarted);
    }

    /**
     * При старте раунда каждый раз у болезней будут новые действующие лекарства,
     * Регулируется это параметром Randomize в прототипе болезней.
     * P.S. Пока не забыл, запишу тут, можно добавить болезням маркер частей тел
     * В которых они могут "обитать" и в которых уже "обитают", для хир.операций
     */
    private void OnRoundStarted(RoundStartedEvent ev)
    {
        var diseasesController = Spawn(DiseasesController);
        var diseasesControllerComp = EnsureComp<DiseasesControllerComponent>(diseasesController);

        var diseases = _prototype.EnumeratePrototypes<DiseasePrototype>();
        var whiteListedReagents = GetWhitelistedReagents();

        foreach (var disease in diseases)
        {
            if (disease.Cure == null || !_prototype.TryIndex(disease.Cure, out var prototype))
                continue;

            var cureData = new DiseaseCureData
            {
                Reagents = prototype.Reagents,
                ExternalReagents = prototype.ExternalReagents,
                VaccinatorReagents = GetVaccinatorReagents(prototype.Reagents, whiteListedReagents),
                Randomize = prototype.Randomize,
            };

            diseasesControllerComp.DiseasesCures[disease.ID] = cureData;
        }
    }

    private List<string> GetWhitelistedReagents()
    {
        if (!_prototype.TryIndex<DiseaseBlacklistPrototype>(DefaultAnalyzeBlackList, out var blacklistedReagents))
            return [];

        var whitelistedReagents = new List<string>();
        var reagents = _prototype.EnumeratePrototypes<ReagentPrototype>().ToList();
        foreach (var reagent in reagents)
        {
            if (blacklistedReagents.BlackListReagents.Contains(reagent.ID))
                continue;

            whitelistedReagents.Add(reagent.ID);
        }

        return whitelistedReagents;
    }

    private List<string> GetVaccinatorReagents(
        List<ProtoId<ReagentPrototype>> validReagents,
        List<string> reagents
    )
    {
        var vaccinatorReagents = new List<string>();
        var tries = 0;
        while (tries < 2)
        {
            _random.Shuffle(reagents);
            var reagent = _random.Pick(reagents);
            if (validReagents.Contains(reagent))
                continue;

            vaccinatorReagents.Add(reagent);
            tries++;
        }

        vaccinatorReagents.Add(_random.Pick(validReagents));
        return vaccinatorReagents;
    }

    public bool CanHealDisease(EntityUid entity, EntityUid target)
    {
        if (!TryComp<DiseaseTargetComponent>(target, out var diseaseTarget))
            return false;

        if (!HasComp<HealingComponent>(entity))
            return false;

        var meta = MetaData(entity);
        if (meta.EntityPrototype?.ID is not { } reagentId)
            return false;

        return CanCureWithReagent((target, diseaseTarget), reagentId);
    }

    public bool TryHealDiseaseExternal(EntityUid entity, EntityUid target)
    {
        if (!TryComp<DiseaseTargetComponent>(target, out var diseaseTarget))
            return false;

        if (!HasComp<HealingComponent>(entity))
            return false;

        var meta = MetaData(entity);
        if (meta.EntityPrototype?.ID is not { } reagentId)
            return false;

        return TryApplyExternalCureReagent((target, diseaseTarget), reagentId);
    }

    private bool TryApplyExternalCureReagent(Entity<DiseaseTargetComponent> target, string reagentId)
    {
        var cureDisease = false;
        var curTime = _gameTiming.CurTime;

        using var enumerator = target.Comp.Diseases.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var key = enumerator.Current.Key;
            var data = enumerator.Current.Value;

            if (CanCureWithReagent(key, reagentId))
                continue;

            cureDisease = true;
            data.CureTime = curTime + data.CureTimeThreshold;
        }

        return cureDisease;
    }

    private void OnReagentEffectApply(Entity<DiseaseTargetComponent> ent, ref ReagentEffectApplyEvent args)
    {
        if (args.Args.Reagent?.ID is not { } reagentId)
            return;

        using var enumerator = ent.Comp.Diseases.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var key = enumerator.Current.Key;
            var data = enumerator.Current.Value;

            if (CanCureWithReagent(key, reagentId))
            {
                RemoveDisease(ent, key);
                continue;
            }

            TryApplyReagentToSymptom(ent, key, data, reagentId);
        }
    }

    private bool CanCureWithReagent(Entity<DiseaseTargetComponent> target, string reagentId)
    {
        using var enumerator = target.Comp.Diseases.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var key = enumerator.Current.Key;
            if (CanCureWithReagent(key, reagentId))
                return true;
        }

        return false;
    }

    private bool CanCureWithReagent(string diseaseId, string reagentId)
    {
        if (GetController() is not { } controller)
            return false;

        if (!controller.Comp.DiseasesCures.TryGetValue(diseaseId, out var diseaseCure))
            return false;

        return diseaseCure.Reagents.Contains(reagentId) || diseaseCure.ExternalReagents.Contains(reagentId);
    }

    public bool CanCureDisease(string diseaseId)
    {
        if (GetController() is not { } controller)
            return false;

        if (!controller.Comp.DiseasesCures.TryGetValue(diseaseId, out var diseaseCure))
            return false;

        return diseaseCure.Reagents.Count != 0 || diseaseCure.ExternalReagents.Count != 0;
    }

    public List<ProtoId<ReagentPrototype>> GetDiseaseCureReagents(string diseaseId)
    {
        if (GetController() is not { } controller)
            return [];

        if (!controller.Comp.DiseasesCures.TryGetValue(diseaseId, out var diseaseCure))
            return [];

        return diseaseCure.Reagents;
    }

    public List<string> GetDiseaseVaccinatorReagents(string diseaseId)
    {
        if (GetController() is not { } controller)
            return [];

        if (!controller.Comp.DiseasesCures.TryGetValue(diseaseId, out var diseaseCure))
            return [];

        return diseaseCure.VaccinatorReagents;
    }

    private Entity<DiseasesControllerComponent>? GetController()
    {
        var query = EntityQueryEnumerator<DiseasesControllerComponent>();
        while (query.MoveNext(out var ent, out var comp))
        {
            return (ent, comp);
        }

        return null;
    }
}

using Content.Server._Adventure.Medical.Disease.Components;
using Content.Server._Adventure.Medical.Disease.Data;
using Content.Server._Adventure.Medical.Disease.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Adventure.Medical.Disease.Systems;

public sealed partial class DiseaseSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookupSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly DiseasesImmunitySystem _immunitySystem = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private bool _isDiseasesEnabled;

    public override void Initialize()
    {
        base.Initialize();

        InitializeCure();
        InitializeSpreading();

        Subs.CVar(_cfg, SecretCCVars.SecretCCVars.IsDeseasesEnabled, value => _isDiseasesEnabled = value, true);
    }

    public void ForceAddDisease(EntityUid uid, string disease)
    {
        if (!_isDiseasesEnabled)
            return;

        if (!_prototype.TryIndex<DiseasePrototype>(disease, out var diseasePrototype))
            return;

        var diseaseData = diseasePrototype.ToDiseaseData(_prototype);
        var comp = EnsureComp<DiseaseTargetComponent>(uid);
        comp.Diseases[disease] = diseaseData;

        SetupSpreading((uid, comp), disease, diseaseData.Spreads);
        SetupDisease((uid, comp), diseaseData);
    }

    private void SetupDisease(Entity<DiseaseTargetComponent> target, DiseaseData diseaseData)
    {
        foreach (var stage in diseaseData.Stages)
        {
            foreach (var effects in stage.Symptom.Effects)
            {
                effects.OnStart(EntityManager, target);
            }
        }
    }

    private void AddDisease(EntityUid uid, string disease)
    {
        if (!_isDiseasesEnabled)
            return;

        if (!TryComp<DiseaseTargetComponent>(uid, out var component))
            return;

        if (component.Diseases.ContainsKey(disease))
            return;

        if (_immunitySystem.HasImmuneForDisease(uid, disease))
            return;

        if (_immunitySystem.IsImmunitySaveFromDisease(uid))
            return;

        ForceAddDisease(uid, disease);
    }

    private void RemoveDisease(Entity<DiseaseTargetComponent> target, string key)
    {
        var targetDisease = target.Comp.Diseases[key];
        foreach (var stage in targetDisease.Stages)
        {
            foreach (var effect in stage.Symptom.Effects)
            {
                effect.Dispose(EntityManager, target);
            }
        }

        target.Comp.Diseases.Remove(key);
        _popupSystem.PopupClient("Вы чувствуете, что вам стало лучше", target, target);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_isDiseasesEnabled)
            return;

        var curTime = _gameTiming.CurTime;
        var query = EntityQueryEnumerator<DiseaseTargetComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.Diseases.Count == 0)
                continue;

            UpdateDiseases((uid, component), curTime);
        }
    }

    private void UpdateDiseases(Entity<DiseaseTargetComponent> target, TimeSpan curTime)
    {
        using var diseases = target.Comp.Diseases.GetEnumerator();
        while (diseases.MoveNext())
        {
            var key = diseases.Current.Key;
            var data = diseases.Current.Value;
            if (data.Stages.Count == 0)
                continue;

            if (data.CurrentStage == null && !GetNextStage(target, data, curTime))
                continue;


            if (IsDiseaseShouldRemoved(target, data, key, curTime))
            {
                RemoveDisease(target, key);
                continue;
            }


            foreach (var stage in data.Stages)
            {
                UpdateDiseaseStage(target, stage, curTime);
                GetNextStage(target, data, curTime);
            }
        }
    }

    private bool IsDiseaseShouldRemoved(EntityUid target, DiseaseData data, string key, TimeSpan curTime)
    {
        if (_immunitySystem.UpdateDiseaseImmunity(target, data, key, curTime))
            return true;

        if (data.CureTime != TimeSpan.Zero && data.CureTime < curTime)
            return true;

        return false;
    }

    private void UpdateDiseaseStage(Entity<DiseaseTargetComponent> target, DiseaseStage stage, TimeSpan curTime)
    {
        if (stage.SymptomTime > curTime || !stage.Active || stage.Cured)
            return;

        foreach (var effect in stage.Symptom.Effects)
        {
            effect.Execute(EntityManager, target);
        }

        stage.SymptomTime = curTime + stage.SymptomThreshold;
    }

    private bool GetNextStage(Entity<DiseaseTargetComponent> target, DiseaseData data, TimeSpan curTime)
    {
        if (data.CurrentStage?.MutationTime > curTime)
            return false;

        var nextStageIndex = data.CurrentStage == null ? 0 : data.Stages.IndexOf(data.CurrentStage) + 1;
        if (nextStageIndex >= data.Stages.Count)
            return false;

        if (data.CurrentStage != null)
            data.CurrentStage.MutationTime = TimeSpan.Zero;

        var nextStage = data.Stages[nextStageIndex];

        nextStage.Active = true;
        nextStage.SymptomTime = curTime + nextStage.SymptomThreshold;
        nextStage.MutationTime = curTime + nextStage.MutationThreshold;

        data.CurrentStage = nextStage;

        return true;
    }
}

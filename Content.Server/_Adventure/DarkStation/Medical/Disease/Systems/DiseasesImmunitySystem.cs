using Content.Server.AdventureSpace.Medical.Disease.Components;
using Content.Server.AdventureSpace.Medical.Disease.Data;
using Content.Shared.FixedPoint;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.AdventureSpace.Medical.Disease.Systems;

public sealed class DiseasesImmunitySystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DiseaseImmunityComponent, ComponentInit>(OnImmunityInit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _gameTiming.CurTime;
        var query = EntityQueryEnumerator<DiseaseImmunityComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            UpdateImmunity((uid, component), curTime);
            UpdateDiseasesImmunity((uid, component), curTime);
        }
    }

    private void OnImmunityInit(Entity<DiseaseImmunityComponent> ent, ref ComponentInit args)
    {
        ent.Comp.ImmunityPenaltyPeriod += _gameTiming.CurTime + ent.Comp.ImmunityPenaltyThreshold;
    }

    public bool HasImmuneForDisease(EntityUid entityUid, string disease)
    {
        if (!TryComp<DiseaseImmunityComponent>(entityUid, out var component))
            return false;

        return HasImmuneForDisease((entityUid, component), disease);
    }

    public bool HasImmuneForDisease(Entity<DiseaseImmunityComponent> entity, string disease)
    {
        if (!entity.Comp.ImmunityDiseasesProgress.TryGetValue(disease, out var diseaseImmunityProgress))
            return false;

        return diseaseImmunityProgress.Immunity >= entity.Comp.BaseMaxImmunity;
    }

    public bool IsImmunitySaveFromDisease(EntityUid entityUid)
    {
        return TryComp<DiseaseImmunityComponent>(entityUid, out var component) &&
               IsImmunitySaveFromDisease((entityUid, component));
    }

    public bool IsImmunitySaveFromDisease(Entity<DiseaseImmunityComponent> entity)
    {
        //Пример: иммунитет 70, а выпало 60, значит человек не заболеет
        //Чем выше иммунитет, тем меньше шансов заболеть
        if (_random.Prob(entity.Comp.ImmunityLevel / 100f))
            return false;

        entity.Comp.ImmunityLevel -= entity.Comp.BaseImmunityPenalties;
        return true;
    }

    private void UpdateImmunity(Entity<DiseaseImmunityComponent> entity, TimeSpan curTime)
    {
        if (entity.Comp.ImmunityPenaltyPeriod > curTime)
            return;

        entity.Comp.ImmunityLevel -= entity.Comp.BaseImmunityPenalties;
        if (entity.Comp.ImmunityLevel < 0f)
            entity.Comp.ImmunityLevel = 0f;

        entity.Comp.ImmunityPenaltyPeriod = curTime + entity.Comp.ImmunityPenaltyThreshold;
    }

    public bool UpdateDiseaseImmunity(EntityUid target,
        DiseaseData data,
        string key,
        TimeSpan curTime)
    {
        if (!TryComp<DiseaseImmunityComponent>(target, out var component))
            return false;

        if (data.ImmunityIncreaseType != ImmunityIncreaseType.Time)
            return false;

        if (data.ImmunityIncreaseTime > curTime)
            return false;

        var immunitiesProgress = component.ImmunityDiseasesProgress;
        if (!immunitiesProgress.TryGetValue(key, out var diseaseImmunity))
        {
            immunitiesProgress.Add(key, new DiseaseImmunityProgress());
            return false;
        }

        diseaseImmunity.Immunity += component.ImmunityLevel * component.BaseImmunityFriction;
        if (diseaseImmunity.Immunity >= component.BaseMaxImmunity)
        {
            diseaseImmunity.Immunity = component.BaseMaxImmunity;
            diseaseImmunity.ImmunityDecreaseTime = curTime + diseaseImmunity.ImmunityDecreaseThreshold;
            diseaseImmunity.CanDecrease = true;

            return true;
        }

        data.ImmunityIncreaseTime = curTime + data.ImmunityIncreaseThreshold;

        return false;
    }

    private void UpdateDiseasesImmunity(Entity<DiseaseImmunityComponent> entity, TimeSpan curTime)
    {
        using var enumerator = entity.Comp.ImmunityDiseasesProgress.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var progress = enumerator.Current.Value;
            if (!progress.CanDecrease || progress.ImmunityDecreaseTime > curTime)
                continue;

            progress.Immunity -= entity.Comp.BaseImmunityPenalties;

            if (progress.Immunity <= 0f)
            {
                entity.Comp.ImmunityDiseasesProgress.Remove(enumerator.Current.Key);
                continue;
            }

            progress.ImmunityDecreaseTime = curTime + progress.ImmunityDecreaseThreshold;
        }
    }

    public void IncreaseImmunityByVitamins(EntityUid uid, FixedPoint2 quantity)
    {
        if (!TryComp<DiseaseImmunityComponent>(uid, out var component))
            return;

        component.ImmunityLevel += quantity.Float() * component.BaseImmunityFriction;

        if (component.ImmunityLevel > component.BaseMaxImmunity)
            component.ImmunityLevel = component.BaseMaxImmunity;
    }
}

using Content.Server._c4llv07e.Bridges;
using Content.Server._Adventure.Medical.Disease.Systems;
using Content.Shared.FixedPoint;

namespace Content.Server._Adventure.Medical.Disease;

public sealed class DiseasesBridge : IDiseasesBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public void TransferDiseasesContact(EntityUid recipient, EntityUid donor)
    {
        _entityManager.System<DiseaseSystem>().TransferDiseasesContact(recipient, donor);
    }

    public bool CanHealDisease(EntityUid entityUid, EntityUid target)
    {
        return _entityManager.System<DiseaseSystem>().CanHealDisease(entityUid, target);
    }

    public bool TryHealDisease(EntityUid entityUid, EntityUid target)
    {
        return _entityManager.System<DiseaseSystem>().TryHealDiseaseExternal(entityUid, target);
    }

    public void IncreaseImmunityByVitamins(EntityUid uid, FixedPoint2 quantity)
    {
        _entityManager.System<DiseasesImmunitySystem>().IncreaseImmunityByVitamins(uid, quantity);
    }
}

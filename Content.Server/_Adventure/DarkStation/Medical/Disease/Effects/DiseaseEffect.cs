using Content.Server.AdventureSpace.Medical.Surgery.Organs.Events;
using Content.Server.Body.Systems;
using Content.Shared.Damage;
using OrganType = Content.Shared.Body.Organ.OrganType;

namespace Content.Server.AdventureSpace.Medical.Disease.Effects;

[ImplicitDataDefinitionForInheritors]
public abstract partial class DiseaseEffect
{
    [DataField]
    public DamageSpecifier? HealthDamage;

    [DataField]
    public Dictionary<OrganType, int>? OrgansDamage;

    public virtual void OnStart(IEntityManager entityManager, EntityUid target)
    {
    }

    public virtual void Execute(IEntityManager entityManager, EntityUid target)
    {
        ApplyBasicDamage(entityManager, target);
        ApplyOrgansDamage(entityManager, target);
    }

    private void ApplyBasicDamage(IEntityManager entityManager, EntityUid target)
    {
        var damageSystem = entityManager.System<DamageableSystem>();
        if (HealthDamage is { } healthDamage)
            damageSystem.TryChangeDamage(target, healthDamage);
    }

    private void ApplyOrgansDamage(IEntityManager entityManager, EntityUid target)
    {
        var bodySystem = entityManager.System<BodySystem>();
        if (OrgansDamage is not { } organDamage)
            return;

        foreach (var (organ, damage) in organDamage)
        {
            foreach (var (uid, component) in bodySystem.GetBodyOrgans(target))
            {
                if (component.OrganType != organ)
                    continue;

                var ev = new SurgeryRequestOrganDamage(damage);
                entityManager.EventBus.RaiseLocalEvent(uid, ref ev);
            }
        }
    }

    public virtual void Dispose(IEntityManager entityManager, EntityUid target)
    {
    }
}

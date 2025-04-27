using Content.Shared.Damage.Systems;

namespace Content.Server.AdventureSpace.Medical.Disease.Effects;

[DataDefinition]
public sealed partial class DiseaseStaminaEffect : DiseaseEffect
{
    [DataField(required: true)]
    public float Damage;

    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);

        var stamina = entityManager.System<StaminaSystem>();
        stamina.TakeStaminaDamage(target, Damage);
    }
}

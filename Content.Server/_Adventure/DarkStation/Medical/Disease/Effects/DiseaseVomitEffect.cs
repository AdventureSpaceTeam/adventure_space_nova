using Content.Server.Medical;

namespace Content.Server._Adventure.Medical.Disease.Effects;

public sealed partial class DiseaseVomitEffect : DiseaseEffect
{
    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);

        var vomitSys = entityManager.System<VomitSystem>();
        vomitSys.Vomit(target);
    }
}

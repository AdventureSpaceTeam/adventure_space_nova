using System.Linq;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Medical;
using Content.Shared.Body.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Random;

namespace Content.Server._Adventure.Medical.Disease.Effects;

[DataDefinition]
public sealed partial class DiseaseVomitRandomOrganEffect : DiseaseEffect
{
    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);

        if (!entityManager.TryGetComponent<BodyComponent>(target, out var bodyComponent))
            return;

        var bodySystem = entityManager.System<BodySystem>();
        var organs = bodySystem.GetBodyOrgans(target).ToList();
        if (!organs.Any())
            return;

        var random = IoCManager.Resolve<IRobustRandom>();
        random.Shuffle(organs);

        var targetOrgan = random.PickAndTake(organs);
        if (entityManager.HasComponent<BrainComponent>(targetOrgan.Id) ||
            entityManager.HasComponent<EyeComponent>(targetOrgan.Id))
            return;

        var transformSystem = entityManager.System<TransformSystem>();
        transformSystem.AttachToGridOrMap(targetOrgan.Id);

        var vomitSystem = entityManager.System<VomitSystem>();
        vomitSystem.Vomit(target);
    }
}

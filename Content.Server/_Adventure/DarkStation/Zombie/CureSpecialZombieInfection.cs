using Content.Server.Polymorph.Systems;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Zombie;

public sealed partial class CureSpecialZombieInfection : EntityEffect
{
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-innoculate-special-zombie-infection");
    }

    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args is not EntityEffectReagentArgs reagentArgs)
            return;

        var entityManager = args.EntityManager;
        var target = reagentArgs.TargetEntity;
        if (!entityManager.HasComponent<SpecialZombieComponent>(target))
            return;

        var polymorphSystem = entityManager.System<PolymorphSystem>();
        polymorphSystem.Revert(target);
    }
}

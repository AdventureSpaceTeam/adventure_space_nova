using Content.Shared.RPSX.DarkForces.Saint.Reagent.Events;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Components;

namespace Content.Shared.RPSX.DarkForces.Saint.Reagent;

public sealed partial class SaintWaterDrinkEffectSystem : EntityEffectSystem<DamageableComponent, SaintWaterDrink>
{
    protected override void Effect(Entity<DamageableComponent> entity, ref EntityEffectEvent<SaintWaterDrink> args)
    {
        var saintWaterDrinkEvent = new OnSaintWaterDrinkEvent(entity);
        RaiseLocalEvent(entity, saintWaterDrinkEvent);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class SaintWaterDrink : EntityEffectBase<SaintWaterDrink>
{
    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "Помогает бороться с нечистью";
    }
}
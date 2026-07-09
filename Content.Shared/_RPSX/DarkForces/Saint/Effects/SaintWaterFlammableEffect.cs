using Content.Shared.RPSX.DarkForces.Saint.Reagent.Events;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Log;
using Content.Shared.Damage.Components;

namespace Content.Shared.RPSX.DarkForces.Saint.Reagent;

public sealed partial class SaintWaterFlammableEffectSystem : EntityEffectSystem<DamageableComponent, SaintWaterFlammable>
{
    protected override void Effect(Entity<DamageableComponent> entity, ref EntityEffectEvent<SaintWaterFlammable> args)
    {
        var saintWaterDrinkEvent = new OnSaintWaterFlammableEvent(entity);
        RaiseLocalEvent(entity, saintWaterDrinkEvent);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class SaintWaterFlammable : EntityEffectBase<SaintWaterFlammable>
{
    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "Помогает бороться с нечистью";
    }
}
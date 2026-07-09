using Content.Shared.EntityEffects;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.DarkForces.Vampire;

public sealed partial class VampireThirstEffectSystem : EntityEffectSystem<ThirstComponent, VampireThirst>
{
    private const float DefaultHydrationFactor = 3.0f;

    [DataField("factor")]
    public float HydrationFactor { get; set; } = DefaultHydrationFactor;

    [Dependency] private readonly ThirstSystem _thirstSystem = default!;

    protected override void Effect(Entity<ThirstComponent> entity, ref EntityEffectEvent<VampireThirst> args)
    {
        var isVampire = HasComp<VampireComponent>(entity);
        if (!isVampire) return;
        _thirstSystem.ModifyThirst(entity, entity.Comp, HydrationFactor);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class VampireThirst : EntityEffectBase<VampireThirst>
{
    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return "";
    }
}
using System.Linq;
using Content.Server.Body.Events;
using Content.Server.Body.Systems;
using Content.Server.Nutrition.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Medical.Surgery.Organs.Lungs;

public sealed class SurgeryLungsSystem : SurgeryOrganSystem<DLungsComponent, LungsMarkerComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DLungsComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<DLungsComponent, OnEntityBreathGas>(OnBreathGas);
        SubscribeLocalEvent<DLungsComponent, OnEntityInhaleToLungs>(OnInhale);
        SubscribeLocalEvent<DLungsComponent, OnEntitySmoke>(OnSmoke);
    }

    private void OnSmoke(Entity<DLungsComponent> ent, ref OnEntitySmoke args)
    {
        ent.Comp.Damage += args.Amount * ent.Comp.DamageMod;
        UpdateOrganStatus(ent);
    }

    private void OnInhale(Entity<DLungsComponent> ent, ref OnEntityInhaleToLungs args)
    {
        var damageLoss = ent.Comp.Damage;
        if (damageLoss > 1.0f)
            damageLoss = 1.0f;

        args.DamageLoss = damageLoss;
    }

    private void OnBreathGas(Entity<DLungsComponent> ent, ref OnEntityBreathGas args)
    {
        if (args.Reagent is not { } reagentId)
            return;

        if (!_prototype.TryIndex<ReagentPrototype>(reagentId, out var proto) || proto.Metabolisms == null)
            return;

        var isIgnore = ent.Comp.IgnoreReagentsToxin.Contains(reagentId);
        var isToxic = ent.Comp.DamageGroups.Any(group => proto.Metabolisms.ContainsKey(group));
        var isBadGas = ent.Comp.DamageGases.Any(gas => gas == proto.ID);

        var toxic = !isIgnore && (isToxic || isBadGas);
        if (!toxic || args.Amount < 0.1f)
            return;

        ent.Comp.Damage += args.Amount * ent.Comp.DamageMod;
        UpdateOrganStatus(ent);
    }

    private void OnComponentInit(Entity<DLungsComponent> ent, ref ComponentInit args)
    {
        ent.Comp.WarningThreshold = 0.05f;
        ent.Comp.CriticalThreshold = 0.18f;
    }
}

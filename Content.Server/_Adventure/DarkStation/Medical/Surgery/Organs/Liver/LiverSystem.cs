using Content.Server.Body.Events;
using Content.Shared.Chemistry.EntitySystems;

namespace Content.Server._Adventure.Medical.Surgery.Organs.Liver;

public sealed class LiverSystem : SurgeryOrganSystem<ToxinFilterComponent, ToxinFilterMarkerComponent>
{
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ToxinFilterComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<ToxinFilterComponent, OnEntityMetabolizeAfterReagent>(OnMetabolizeAfter);
        SubscribeLocalEvent<ToxinFilterComponent, OnEntityAfterMetabolize>(OnAfterMetabolize);
    }

    private void OnAfterMetabolize(Entity<ToxinFilterComponent> ent, ref OnEntityAfterMetabolize args)
    {
        //If no toxin filter (liver) add toxin reagent (by scale determined by UnfilteredToxinRate from MetabolizerComponent (0.1))
        if (ent.Comp.Working)
            return;

        _solutionContainerSystem.TryAddReagent(
            args.Solution,
            ent.Comp.UnfilteredToxinReagent,
            args.MostToRemove * ent.Comp.UnfilteredToxinRate,
            out _
        );
    }

    private void OnMetabolizeAfter(Entity<ToxinFilterComponent> ent, ref OnEntityMetabolizeAfterReagent args)
    {
        if (!ent.Comp.FilterToxins.Contains(args.Entry.Id))
            return;

        ent.Comp.Damage += args.Scale;
        UpdateOrganStatus(ent);
    }

    private void OnComponentInit(EntityUid uid, ToxinFilterComponent component, ComponentInit args)
    {
        component.WarningThreshold = component.MaxDamageThreshold * 0.5f;
        component.CriticalThreshold = component.MaxDamageThreshold * 0.75f;
    }
}

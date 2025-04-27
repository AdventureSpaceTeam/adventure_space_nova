using Content.Server.Body.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Medical.Disease.Effects;

[DataDefinition]
public sealed partial class DiseaseAdjustReagentEffect : DiseaseEffect
{
    [DataField(required: true)]
    public FixedPoint2 Amount;

    [DataField(required: true)]
    public ProtoId<ReagentPrototype> Reagent;

    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);

        if (!entityManager.TryGetComponent<BloodstreamComponent>(target, out var bloodstream))
            return;

        if (!entityManager.TryGetComponent<SolutionComponent>(target, out var solutionComponent))
            return;

        var stream = bloodstream.ChemicalSolution;
        if (stream?.Comp.Solution is not { } solution)
            return;

        var solutionSys = entityManager.System<SharedSolutionContainerSystem>();

        var reagentId = new ReagentId(Reagent.Id, null);
        if (Amount < 0)
        {
            var reagentQuantity = solution.GetReagentQuantity(reagentId);
            solutionSys.RemoveReagent((target, solutionComponent), reagentId, reagentQuantity);
        }

        if (Amount > 0)
            solutionSys.TryAddReagent(stream.Value, Reagent, Amount, out _);
    }
}

using System.Linq;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Server.Doors.Systems;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Doors.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Content.Shared.Maps;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using JetBrains.Annotations;
using Robust.Shared.Map;

namespace Content.Server.AdventureSpace.GameRules.SCP.SCP173;

[UsedImplicitly]
[DataDefinition]
public sealed partial class SCP173TileReaction : ITileReaction
{
    public FixedPoint2 TileReact(TileRef tile,
        ReagentPrototype reagent,
        FixedPoint2 reactVolume,
        IEntityManager entityManager)
    {
        var lookupSystem = entityManager.System<EntityLookupSystem>();
        var turf = entityManager.System<TurfSystem>();
        var entityCoordinates = turf.GetTileCenter(tile);

        var entitiesInRange = lookupSystem.GetEntitiesInRange(entityCoordinates, 3);
        TryOpenDoor(entityManager, entitiesInRange);

        var spillSystem = entityManager.System<PuddleSystem>();
        if (reactVolume < 5 || !spillSystem.TryGetPuddle(tile, out _))
            return FixedPoint2.Zero;

        return spillSystem.TrySpillAt(tile, new Solution(reagent.ID, reactVolume), out _, false, false)
            ? reactVolume
            : FixedPoint2.Zero;
    }

    private int GetSCPBloodVolume(IEntityManager entityManager, HashSet<EntityUid> entitiesInRange)
    {
        var solutionContainerSystem = entityManager.System<SolutionContainerSystem>();
        var puddles = entitiesInRange.Where(entityManager.HasComponent<PuddleComponent>);
        var volume = 0;
        var reagentId = new ReagentId("SCP173Blood", null);
        foreach (var puddle in puddles)
        {
            if (!entityManager.TryGetComponent<SolutionContainerManagerComponent>(puddle, out var solutionContainer))
                continue;

            if (!solutionContainerSystem.TryGetSolution((puddle, solutionContainer), "puddle", out var puddleSolution))
                continue;

            if (!puddleSolution.Value.Comp.Solution.TryGetReagent(reagentId, out var quantity))
                continue;

            volume += quantity.Quantity.Int();
        }

        return volume;
    }

    private void TryOpenDoor(IEntityManager entityManager, HashSet<EntityUid> entitiesInRange)
    {
        var volumesAround = GetSCPBloodVolume(entityManager, entitiesInRange);
        if (volumesAround < 1700)
            return;

        var doorSystem = entityManager.System<DoorSystem>();
        var weldableSystem = entityManager.System<WeldableSystem>();
        var doors = entitiesInRange.Where(entityManager.HasComponent<DoorComponent>);

        foreach (var door in doors)
        {
            if (!entityManager.TryGetComponent<DoorComponent>(door, out var doorComp) || doorComp == null)
                continue;

            if (entityManager.TryGetComponent<WeldableComponent>(door, out var weldableComp) && weldableComp.IsWelded)
                weldableSystem.SetWeldedState(door, false);

            doorSystem.TryOpen(door, doorComp, quiet: true);
        }
    }
}

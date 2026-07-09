using System.Linq;
using Content.Server.RPSX.DarkForces.Narsi.Buildings.CreatureEgg;
using Content.Shared.Objectives.Components;
using Content.Shared.RPSX.DarkForces.Narsi.Progress.Objectives;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Egg;

public sealed class NarsiCultCreatureEggSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NarsiCultCreatureEggObjectiveComponent, GroupObjectiveAssignedEvent>(OnAssigned);
        SubscribeLocalEvent<NarsiCreatureEggSpawnEvent>(OnCreatureEggSpawnEvent);
    }

    private void OnCreatureEggSpawnEvent(NarsiCreatureEggSpawnEvent ev)
    {
        var query = EntityQuery<NarsiCultCreatureEggObjectiveComponent>().ToList();
        foreach (var component in query)
        {
            if (component.CreatureId?.Id != ev.CreatureId)
                continue;

            RaiseLocalEvent(new NarsiCultObjectiveCompleted(component.Owner));
        }
    }

    private void OnAssigned(EntityUid uid, NarsiCultCreatureEggObjectiveComponent component, ref GroupObjectiveAssignedEvent args)
    {
        var targetCreature = _robustRandom.Pick(component.AvailableCreatures);
        if (!_prototypeManager.TryIndex(targetCreature, out var targetCreaturePrototype))
        {
            args.Cancelled = true;
            return;
        }

        component.CreatureId = targetCreature;
        _metaData.SetEntityName(uid, $"Вырастите сущность: {targetCreaturePrototype.Name}");
    }
}

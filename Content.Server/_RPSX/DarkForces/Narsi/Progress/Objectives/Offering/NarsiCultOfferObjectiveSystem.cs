using System.Linq;
using Content.Server.RPSX.DarkForces.Narsi.Progress.Components;
using Content.Server.RPSX.DarkForces.Saint.Chaplain.Components;
using Content.Server.Mind;
using Content.Shared.Objectives.Components;
using Content.Shared.Roles.Jobs;
using Content.Shared.RPSX.DarkForces.Narsi.Progress.Objectives;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Content.Shared.Mind;
using Content.Server.RPSX.Helpers;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Offering;

public sealed class NarsiCultOfferObjectiveSystem : EntitySystem
{
    [Dependency] private readonly SharedJobSystem _job = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly MindHelpers _mindHelpers = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NarsiCultOfferObjectiveComponent, GroupObjectiveAssignedEvent>(OnAssigned);
        SubscribeLocalEvent<NarsiCultOfferingTargetComponent, NarsiCultOfferingTargetEvent>(OnOfferingTarget);
    }

    private void OnOfferingTarget(EntityUid uid, NarsiCultOfferingTargetComponent component, ref NarsiCultOfferingTargetEvent args)
    {
        foreach (var objective in component.Objectives)
        {
            RaiseLocalEvent(new NarsiCultObjectiveCompleted(objective));
        }
    }

    private void OnAssigned(EntityUid uid, NarsiCultOfferObjectiveComponent component, ref GroupObjectiveAssignedEvent args)
    {
        var allHumans = _mind.GetAliveHumans()
            .Where(entity => !HasComp<NarsiCultistComponent>(entity.Comp.OwnedEntity) && !HasComp<ChaplainComponent>(entity.Comp.OwnedEntity))
            .ToList();

        if (allHumans.Count == 0)
        {
            args.Cancelled = true;
            return;
        }

        var target = _random.Pick(allHumans);
        var objective = (uid, component);
        if (target.Comp.OwnedEntity == null) return;
        SetupOfferingTarget(objective, target.Comp.OwnedEntity.Value);

        var title = GetObjectiveTitle(objective, target);
        _metaData.SetEntityName(uid, title);
    }

    private void SetupOfferingTarget(Entity<NarsiCultOfferObjectiveComponent> objective, EntityUid target)
    {
        var targetComp = EnsureComp<NarsiCultOfferingTargetComponent>(target);
        targetComp.Objectives.Add(objective);

        objective.Comp.Target = target;
    }

    private string GetObjectiveTitle(Entity<NarsiCultOfferObjectiveComponent> objective, EntityUid target)
    {
        var objectiveMeta = MetaData(objective);
        var targetName = "Неизвестно";

        if (TryComp<MindComponent>(target, out var mind) && mind.CharacterName != null)
        {
            targetName = mind.CharacterName;
        }

        var jobName = _job.MindTryGetJobName(target);
        return $"{objectiveMeta.EntityName}: {targetName} ({jobName})";
    }
}

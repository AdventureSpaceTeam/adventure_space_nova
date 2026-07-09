using Content.Server.RPSX.DarkForces.Vampire.Role.Components;
using Content.Server.RPSX.DarkForces.Vampire.Role.Trall;
using Content.Shared.Objectives.Components;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.RPSX.DarkForces.Vampire.Rule.Objectives.Enthrall;

public sealed class VampireEnthrallObjectiveSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireEnthrallObjectiveComponent, ObjectiveAssignedEvent>(OnObjectiveAssigned);
        SubscribeLocalEvent<VampireEnthrallObjectiveComponent, ObjectiveAfterAssignEvent>(OnAfterObjectiveAssigned);
        SubscribeLocalEvent<VampireEnthrallObjectiveComponent, ObjectiveGetProgressEvent>(OnGetProgress);
    }

    private void OnObjectiveAssigned(EntityUid uid, VampireEnthrallObjectiveComponent component,
        ref ObjectiveAssignedEvent args)
    {
        component.TrallCount = _robustRandom.Next(2, 4);
    }

    private void OnAfterObjectiveAssigned(EntityUid uid, VampireEnthrallObjectiveComponent component,
        ref ObjectiveAfterAssignEvent args)
    {
        _metaData.SetEntityName(uid,
            Loc.GetString("vampire-enthrall-objective-title", ("trallCount", component.TrallCount)));
    }

    private void OnGetProgress(EntityUid uid, VampireEnthrallObjectiveComponent component,
        ref ObjectiveGetProgressEvent args)
    {
        args.Progress = 0f;

        var entity = args.Mind.CurrentEntity;
        if (entity == null)
            return;

        if (!TryComp<VampireComponent>(entity.Value, out var vampireComponent))
            return;

        if (vampireComponent.FullPower)
        {
            args.Progress = 1f;
            return;
        }

        var trallCount = 0;
        var query = EntityQueryEnumerator<VampireTrallComponent>();
        while (query.MoveNext(out _, out var trallComponent))
        {
            if (trallComponent.OwnerUid != entity)
                continue;

            trallCount++;
        }

        args.Progress = trallCount / component.TrallCount;
    }
}

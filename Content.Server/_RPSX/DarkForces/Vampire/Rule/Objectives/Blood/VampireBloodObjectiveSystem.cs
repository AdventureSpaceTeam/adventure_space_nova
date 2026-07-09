using System;
using Content.Server.RPSX.DarkForces.Vampire.Role.Components;
using Content.Server.Mind;
using Content.Shared.Objectives.Components;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.RPSX.DarkForces.Vampire.Rule.Objectives.Blood;

public sealed class VampireBloodObjectiveSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireBloodObjectiveComponent, ObjectiveAssignedEvent>(OnObjectiveAssigned);
        SubscribeLocalEvent<VampireBloodObjectiveComponent, ObjectiveAfterAssignEvent>(OnAfterObjectiveAssigned);
        SubscribeLocalEvent<VampireBloodObjectiveComponent, ObjectiveGetProgressEvent>(OnGetProgress);
    }

    private void OnObjectiveAssigned(EntityUid uid, VampireBloodObjectiveComponent component,
        ref ObjectiveAssignedEvent args)
    {

        var count = _mindSystem.GetAliveHumans().Count;
        component.RequiredBloodCount = count * 50;
    }

    private void OnAfterObjectiveAssigned(EntityUid uid, VampireBloodObjectiveComponent component,
        ref ObjectiveAfterAssignEvent args)
    {
        _metaData.SetEntityName(uid,
            Loc.GetString("vampire-blood-objective-title", ("bloodCount", component.RequiredBloodCount)));
    }

    private void OnGetProgress(EntityUid uid, VampireBloodObjectiveComponent component,
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

        args.Progress = vampireComponent.TotalDrunkBlood / component.RequiredBloodCount;
    }
}

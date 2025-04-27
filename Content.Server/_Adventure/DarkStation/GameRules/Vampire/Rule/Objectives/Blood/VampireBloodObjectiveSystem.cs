using Content.Shared._Adventure.DarkForces.Vampire.Components;
using Content.Shared.Objectives.Components;
using Robust.Shared.Configuration;
using Robust.Shared.Random;

namespace Content.Server._Adventure.GameRules.Vampire.Rule.Objectives.Blood;

public sealed class VampireBloodObjectiveSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireBloodObjectiveComponent, ObjectiveAssignedEvent>(OnObjectiveAssigned);
        SubscribeLocalEvent<VampireBloodObjectiveComponent, ObjectiveAfterAssignEvent>(OnAfterObjectiveAssigned);
        SubscribeLocalEvent<VampireBloodObjectiveComponent, ObjectiveGetProgressEvent>(OnGetProgress);
    }

    private void OnObjectiveAssigned(EntityUid uid,
        VampireBloodObjectiveComponent component,
        ref ObjectiveAssignedEvent args)
    {
        var min = _cfg.GetCVar(SecretCCVars.SecretCCVars.VampireBloodObjectiveMin);
        var max = _cfg.GetCVar(SecretCCVars.SecretCCVars.VampireBloodObjectiveMax);

        var count = _robustRandom.Next(min, max);
        component.RequiredBloodCount = Math.DivRem(count, 100).Quotient * 100;
    }

    private void OnAfterObjectiveAssigned(EntityUid uid,
        VampireBloodObjectiveComponent component,
        ref ObjectiveAfterAssignEvent args)
    {
        _metaData.SetEntityName(uid,
            Loc.GetString("vampire-blood-objective-title", ("bloodCount", component.RequiredBloodCount)));
    }

    private void OnGetProgress(EntityUid uid,
        VampireBloodObjectiveComponent component,
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

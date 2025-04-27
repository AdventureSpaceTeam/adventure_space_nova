using Content.Server._Adventure.Medical.Surgery.Components;
using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Shared.Body.Part;

namespace Content.Server._Adventure.Medical.Surgery;

public sealed partial class SurgerySystem
{
    private void InitializeClamp()
    {
        SubscribeLocalEvent<SurgeryClampedComponent, ComponentInit>(OnBodyPartClamped);
        SubscribeLocalEvent<SurgeryClampedComponent, SurgeryBodyPartRemovedEvent>(OnClampedPartRemoved);
        SubscribeLocalEvent<SurgeryClampedComponent, SurgeryToolRemovedFromBodyPart>(OnClampedPartRemovedTool);
        SubscribeLocalEvent<SurgeryNecrosisComponent, SurgeryAttachPartAttemptEvent>(OnAttachAttempt);
    }

    private void OnAttachAttempt(Entity<SurgeryNecrosisComponent> ent, ref SurgeryAttachPartAttemptEvent args)
    {
        args.Canceled = true;
        args.Reason = "Эта часть тела сгнила";
    }

    private void OnClampedPartRemovedTool(EntityUid uid,
        SurgeryClampedComponent component,
        SurgeryToolRemovedFromBodyPart args)
    {
        RemoveClamp((uid, component));
    }

    private void OnClampedPartRemoved(EntityUid uid,
        SurgeryClampedComponent component,
        ref SurgeryBodyPartRemovedEvent args)
    {
        RemoveClamp((uid, component));
    }

    private void RemoveClamp(Entity<SurgeryClampedComponent> entity)
    {
        if (TryComp<SurgeryNecrosisComponent>(entity, out var necrosis) && !necrosis.Permanent)
            RemComp<SurgeryNecrosisComponent>(entity);

        RemComp<SurgeryClampedComponent>(entity);
    }

    private void OnBodyPartClamped(EntityUid uid, SurgeryClampedComponent component, ComponentInit args)
    {
        component.NecrosisStartTick = _gameTiming.CurTime + component.NecrosisThreshold;
    }

    private void UpdateClamped(TimeSpan curTime)
    {
        var clampedQuery = EntityQueryEnumerator<SurgeryClampedComponent>();
        while (clampedQuery.MoveNext(out var uid, out var clamped))
        {
            if (clamped.NecrosisStartTick > curTime)
                continue;

            EnsureComp<SurgeryNecrosisComponent>(uid);
        }

        var necrosisQuery = EntityQueryEnumerator<SurgeryNecrosisComponent>();
        while (necrosisQuery.MoveNext(out var uid, out var necrosis))
        {
            if (necrosis.NecrosisPeriodTick > curTime)
                continue;

            necrosis.NecrosisPeriodTick = curTime + necrosis.NecrosisPeriodThreshold;
            necrosis.NecrosisTimes++;

            DoNecrosisDamage((uid, necrosis));
        }
    }

    private void DoNecrosisDamage(Entity<SurgeryNecrosisComponent> bodyPart)
    {
        if (!TryComp<BodyPartComponent>(bodyPart, out var bodyPartComp) || bodyPartComp.Body is not { } body)
            return;

        _damageableSystem.TryChangeDamage(body, bodyPart.Comp.NecrosisDamage, true);

        if (bodyPart.Comp.NecrosisTimes >= bodyPart.Comp.NecrosisMaxTimes)
        {
            _popupSystem.PopupEntity(
                Loc.GetString("surgery-necrosis-permanent", ("entity", MetaData(bodyPart).EntityName)),
                body,
                body);
            bodyPart.Comp.Permanent = true;
            return;
        }

        _popupSystem.PopupEntity(Loc.GetString("surgery-warn-necrosis"), body, body);
    }
}

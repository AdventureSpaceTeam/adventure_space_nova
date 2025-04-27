using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Server._Adventure.Medical.Surgery.Tools.Components;
using Content.Shared._Adventure.Medical.Surgery.Components;
using Content.Shared._Adventure.Medical.Surgery.Events.BodyParts;

namespace Content.Server._Adventure.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeHardSuture()
    {
        SubscribeLocalEvent<SurgeryHardSutureComponent, SurgeryToolApplyEvent>(OnHardSutureApply);
        SubscribeLocalEvent<SurgeryHardSutureComponent, SurgeryHardSutureEndoPart>(OnSutureEndoBodyPart);
        SubscribeLocalEvent<SurgeryHardSutureComponent, SurgeryHardSutureExoPart>(OnSutureExoBodyPart);
    }

    private void OnSutureExoBodyPart(EntityUid uid, SurgeryHardSutureComponent component, SurgeryHardSutureExoPart args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        partEntity.Comp.State.ExoOpened = false;

        PlaySoundWithPopup(args.User, uid, "surgery-exoskeleton-closed");
        UpdatePartStatus(partEntity);
        UpdateTarget(args.Target, SurgeryToolUsage.HardSuture);

        args.Handled = true;
    }

    private void OnSutureEndoBodyPart(EntityUid uid,
        SurgeryHardSutureComponent component,
        SurgeryHardSutureEndoPart args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        partEntity.Comp.State.EndoOpened = false;

        PlaySoundWithPopup(args.User, uid, "surgery-endoskeleton-closed");
        UpdatePartStatus(partEntity);
        UpdateTarget(args.Target, SurgeryToolUsage.HardSuture);

        args.Handled = true;
    }

    private void OnHardSutureApply(EntityUid uid, SurgeryHardSutureComponent component, SurgeryToolApplyEvent args)
    {
        if (args.Handled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        if (partEntity.Comp.State is { Incisable: true, EndoOpened: true })
        {
            HardSutureEndo(partEntity, args);
            return;
        }

        if (!partEntity.Comp.State.ExoOpened)
            return;

        var @event = new SurgeryHardSutureExoPart(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }

    private void HardSutureEndo(Entity<SurgeryBodyPartComponent> entity, SurgeryToolApplyEvent args)
    {
        if (!BodyPartHasAttachment<SurgeryRetractorComponent>(entity.Comp))
        {
            var user = GetEntity(args.Model.User);
            _popupSystem.PopupEntity(Loc.GetString("surgery-no-retractor"), user, user);
            return;
        }

        var @event = new SurgeryHardSutureEndoPart(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }
}

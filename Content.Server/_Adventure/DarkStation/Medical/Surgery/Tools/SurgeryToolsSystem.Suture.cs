using Content.Server._Adventure.Medical.Surgery.Components;
using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Server._Adventure.Medical.Surgery.Tools.Components;
using Content.Shared._Adventure.Medical.Surgery.Events.BodyParts;
using Content.Shared._Adventure.Medical.Surgery.Events.Organs;
using Content.Shared.Body.Organ;
using Content.Shared.Hands.Components;

namespace Content.Server._Adventure.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeSuture()
    {
        SubscribeLocalEvent<SurgerySutureComponent, SurgeryToolApplyEvent>(OnSutureApply);
        SubscribeLocalEvent<SurgerySutureComponent, SurgeryToolApplyOrganEvent>(OnSutureOrganApply);

        SubscribeLocalEvent<SurgerySutureComponent, SurgeryBodyPartDoAfter>(OnIncisionPart);
        SubscribeLocalEvent<SurgerySutureComponent, SurgerySutureOrgan>(OnSutureOrgan);
    }

    private void OnSutureOrgan(EntityUid uid, SurgerySutureComponent component, SurgerySutureOrgan args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var organ = GetEntity(args.NewOrgan);
        var slot = args.Model.Slot;

        if (!TryComp(organ, out OrganComponent? organComp) || !TryComp(args.Target, out SurgeryComponent? surgery))
            return;

        if (!_bodySystem.InsertOrgan(GetEntity(slot.Parent), organ, slot.Id))
            return;

        if (!surgery.CompatibleSpecies.Contains(organComp.Species))
            EnsureComp<SurgeryRejectionComponent>(organ);

        PlaySoundWithPopup(args.User, uid, "surgery-organ-attached");
        UpdateTarget(args.Target, SurgeryToolUsage.Suture);
    }

    private void OnIncisionPart(EntityUid uid, SurgerySutureComponent component, SurgeryBodyPartDoAfter args)
    {
        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        partEntity.Comp.State.Incised = false;

        PlaySoundWithPopup(args.User, uid, "surgery-incision-closed");
        UpdatePartStatus(partEntity);
        UpdateTarget(args.Target, SurgeryToolUsage.Suture);
    }

    private void OnSutureApply(EntityUid uid, SurgerySutureComponent component, SurgeryToolApplyEvent args)
    {
        if (args.Handled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        if (!partEntity.Comp.State.Incised)
            return;

        var user = GetEntity(args.Model.User);
        if (partEntity.Comp.State is { EndoSkeleton: true, EndoOpened: true })
        {
            _popupSystem.PopupEntity(Loc.GetString("surgery-endo-skeleton-opened"), user, user);
            return;
        }

        if (BodyPartHasAttachment<SurgeryRetractorComponent>(partEntity))
        {
            _popupSystem.PopupEntity(Loc.GetString("surgery-retractor-block-stitch"), user, user);
            return;
        }

        var @event = new SurgeryBodyPartDoAfter(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }

    private void OnSutureOrganApply(EntityUid uid, SurgerySutureComponent component, SurgeryToolApplyOrganEvent args)
    {
        if (args.Handled)
            return;

        var user = GetEntity(args.Model.User);
        var slot = args.Model.Slot;

        if (!TryComp<HandsComponent>(user, out var hands) || slot.Child is not null)
            return;

        foreach (var hand in hands.Hands.Values)
        {
            if (!TryComp<OrganComponent>(hand.HeldEntity, out var organComponent))
                continue;

            if (slot.Type != organComponent.OrganType)
                continue;

            var @event = new SurgerySutureOrgan(args.Model, GetNetEntity(hand.HeldEntity.Value));
            StartDoAfter(@event, args.Model);

            args.Handled = true;
            return;
        }
    }
}

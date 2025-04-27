using Content.Server.AdventureSpace.Medical.Surgery.Events;
using Content.Server.AdventureSpace.Medical.Surgery.Tools.Components;
using Content.Shared.AdventureSpace.Medical.Surgery.Components;
using Content.Shared.AdventureSpace.Medical.Surgery.Events.BodyParts;
using Content.Shared.Hands.Components;
using Robust.Shared.Containers;

namespace Content.Server.AdventureSpace.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeRetractor()
    {
        SubscribeLocalEvent<SurgeryRetractorComponent, SurgeryBodyPartDoAfter>(OnRetractorBodyPart);
        SubscribeLocalEvent<SurgeryRetractorComponent, SurgeryToolApplyEvent>(OnRetractorApply);
    }

    private void OnRetractorBodyPart(EntityUid uid, SurgeryRetractorComponent component, SurgeryBodyPartDoAfter args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        AttachToolToPart(uid, args.User, partEntity);

        PlaySoundWithPopup(args.User, uid, "surgery-retractor-applied");
        UpdatePartStatus(partEntity);
        UpdateTarget(args.Target, SurgeryToolUsage.Retractor);
    }

    private void OnRetractorApply(EntityUid uid, SurgeryRetractorComponent component, SurgeryToolApplyEvent args)
    {
        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        var user = GetEntity(args.Model.User);
        if (!partEntity.Comp.State.Incisable)
            return;

        if (partEntity.Comp.State.Attachment != null)
        {
            _popupSystem.PopupEntity(Loc.GetString("surgery-part-already-has-attachment"), user, user);
            return;
        }

        if (!partEntity.Comp.State.Incised)
        {
            _popupSystem.PopupEntity(Loc.GetString("surgery-part-not-incised-retractor"), user, user);
            return;
        }

        var @event = new SurgeryBodyPartDoAfter(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }

    private void AttachToolToPart(EntityUid tool, EntityUid user, Entity<SurgeryBodyPartComponent> entity)
    {
        if (!TryComp<HandsComponent>(user, out var userHands))
            return;

        if (userHands.ActiveHand?.HeldEntity is null ||
            !_handsSystem.TryDrop(user, userHands.ActiveHand, handsComp: userHands))
            return;

        var attachmentContainer = _containerSystem.EnsureContainer<Container>(entity, "attachment");
        _containerSystem.Insert(tool, attachmentContainer);

        entity.Comp.State.Attachment ??= GetNetEntity(tool);
    }
}

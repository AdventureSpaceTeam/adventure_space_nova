using Content.Server.AdventureSpace.Medical.Surgery.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Events;
using Content.Server.AdventureSpace.Medical.Surgery.Tools.Components;
using Content.Shared.AdventureSpace.Medical.Surgery.Components;
using Content.Shared.AdventureSpace.Medical.Surgery.Events.BodyParts;
using Content.Shared.Body.Part;
using Content.Shared.Hands.Components;

namespace Content.Server.AdventureSpace.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeDrill()
    {
        SubscribeLocalEvent<SurgeryDrillComponent, SurgeryAttachBodyPart>(OnDrillBodyPart);
        SubscribeLocalEvent<SurgeryDrillComponent, SurgeryToolApplyEvent>(OnDrillApply);
    }

    private void OnDrillBodyPart(EntityUid uid, SurgeryDrillComponent component, SurgeryAttachBodyPart args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var newPartUid = GetEntity(args.NewBodyPartUid);

        var slot = args.Model.Slot;
        var slotParent = GetEntity(slot.Parent);
        var slotId = slot.Id;

        if (!TryComp<SurgeryBodyPartComponent>(newPartUid, out var bodyPart))
            return;

        if (!TryComp<SurgeryComponent>(GetEntity(args.Model.Target), out var surgery))
            return;

        if (!_bodySystem.AttachPart(slotParent, slotId, newPartUid))
            return;

        if (!surgery.CompatibleSpecies.Contains(bodyPart.Species))
            EnsureComp<SurgeryRejectionComponent>(newPartUid);

        PlaySoundWithPopup(args.User, uid, "surgery-body-part-attached");
        UpdateTarget(args.Target, SurgeryToolUsage.Drill);
    }

    private void OnDrillApply(EntityUid uid, SurgeryDrillComponent component, SurgeryToolApplyEvent args)
    {
        if (args.Handled)
            return;

        var slot = args.Model.Slot;
        if (slot.BodyPart != null)
            return;

        var user = GetEntity(args.Model.User);
        if (!TryComp<HandsComponent>(user, out var hands))
            return;

        foreach (var hand in hands.Hands.Values)
        {
            if (hand.HeldEntity is not { } heldEntity ||
                !TryComp<SurgeryBodyPartComponent>(hand.HeldEntity, out var heldPart))
                continue;

            if (!IsPartValidToAttach(GetEntity(args.Model.User), (heldEntity, heldPart), slot))
                continue;

            var @event = new SurgeryAttachBodyPart(args.Model, GetNetEntity(heldEntity));
            StartDoAfter(@event, args.Model);

            args.Handled = true;
            return;
        }
    }

    private bool IsPartValidToAttach(EntityUid user, Entity<SurgeryBodyPartComponent> entity, BodyPartSlot slot)
    {
        if (!TryComp<BodyPartComponent>(entity, out var bodyPart) || bodyPart.PartType != slot.Type)
        {
            _popupSystem.PopupEntity("Это сюда не подходит", user, user);
            return false;
        }

        if (bodyPart.Symmetry != slot.Symmetry)
        {
            _popupSystem.PopupEntity("Это не симметрично!", user, user);
            return false;
        }

        var ev = new SurgeryAttachPartAttemptEvent();
        RaiseLocalEvent(entity, ref ev);

        if (!ev.Canceled)
            return true;

        _popupSystem.PopupEntity(ev.Reason, user, user);
        return false;
    }
}

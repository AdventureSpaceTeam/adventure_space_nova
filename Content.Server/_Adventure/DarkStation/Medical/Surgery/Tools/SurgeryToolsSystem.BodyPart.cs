using System.Diagnostics.CodeAnalysis;
using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Server._Adventure.Medical.Surgery.Tools.Components;
using Content.Shared._Adventure.Medical.Surgery.Components;
using Content.Shared._Adventure.Medical.Surgery.Events;
using Content.Shared._Adventure.Medical.Surgery.Events.BodyParts;
using Content.Shared.Body.Part;
using Content.Shared.Hands.Components;

namespace Content.Server._Adventure.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    public void ApplyToolOnBodyPartSlot(EntityUid user, EntityUid target, EntityUid? handEntity, BodyPartSlot slot)
    {
        if (handEntity is not { } tool || !HasComp<SurgeryToolComponent>(tool))
            return;

        //Body Part достается из слота, когда работаем со слотом!
        //Если работаем с отдельной частью тела, то не обращаемся сюда. И Все
        var model = new SurgeryBodyPartModel(
            User: GetNetEntity(user),
            Target: GetNetEntity(target),
            Tool: GetNetEntity(tool),
            Slot: slot
        );

        var surgeryEvent = new SurgeryToolApplyEvent(model);
        RaiseLocalEvent(tool, surgeryEvent);
    }

    public void ApplyEmptyHandsOnBodyPart(EntityUid user, EntityUid target, BodyPartSlot slot)
    {
        var bodyPart = GetEntity(slot.BodyPart);
        if (HandleAttachmentRemove(bodyPart, user, target, slot))
            return;

        var slotAttach = GetEntity(slot.Attachment);
        if (!TryComp<SurgeryToolComponent>(slotAttach, out var slotAttachComp))
            return;

        var removePartSlotTool = new SurgeryRemoveToolSlotDoAfter(slot);
        StartDoAfter(removePartSlotTool, user, target, (slotAttach.Value, slotAttachComp));
    }

    private bool HandleAttachmentRemove(EntityUid? part, EntityUid user, EntityUid target, BodyPartSlot slot)
    {
        if (!TryComp<SurgeryBodyPartComponent>(part, out var partComp))
            return false;

        var attachment = GetEntity(partComp.State.Attachment);
        if (!TryComp<SurgeryToolComponent>(attachment, out var attachComp))
            return false;

        var removeBodyPartTool = new SurgeryRemoveToolDoAfter(slot.BodyPart!.Value);
        StartDoAfter(removeBodyPartTool, user, target, (attachment.Value, attachComp));

        return true;
    }

    private void AttachToolToPartSlot(EntityUid tool, EntityUid user, BodyPartSlot slot)
    {
        if (!TryComp<HandsComponent>(user, out var userHands))
            return;

        if (userHands.ActiveHand?.HeldEntity is null ||
            !_handsSystem.TryDrop(user, userHands.ActiveHand, handsComp: userHands))
            return;

        if (!TryComp<BodyPartComponent>(GetEntity(slot.Parent), out var part))
            return;

        part.Childs[slot.Id].Attachment ??= GetNetEntity(tool);
        InsertToolTo(slot.Parent, tool);
    }

    private void OnRemoveToolFromSlot(EntityUid uid, SurgeryToolComponent component, SurgeryRemoveToolSlotDoAfter args)
    {
        var slot = args.Slot;
        if (!TryComp<BodyPartComponent>(GetEntity(slot.Parent), out var part))
            return;

        part.Childs[slot.Id].Attachment = null;

        _handsSystem.PickupOrDrop(args.User, uid);

        RemoveToolFrom(args.Slot.Parent, uid);
        UpdateTarget(args.Target, SurgeryToolUsage.None);

        var bodyPart = GetEntity(args.Slot.BodyPart);
        if (bodyPart == null)
            return;

        var @event = new SurgeryToolRemovedFromBodyPart(args.Slot);
        RaiseLocalEvent(bodyPart.Value, @event);
    }

    private void OnRemoveToolFromPart(EntityUid uid, SurgeryToolComponent component, SurgeryRemoveToolDoAfter args)
    {
        var partUid = GetEntity(args.BodyPart);
        if (!TryComp<SurgeryBodyPartComponent>(partUid, out var partComp))
            return;

        partComp.State.Attachment = null;
        _handsSystem.PickupOrDrop(args.User, uid);

        UpdatePartStatus((partUid, partComp));
        UpdateTarget(args.Target, SurgeryToolUsage.None);
    }

    private bool TryGetBodyPart(SurgeryBodyPartModel model,
        [NotNullWhen(true)] out Entity<SurgeryBodyPartComponent>? entity)
    {
        entity = null;

        var partUid = GetEntity(model.Slot.BodyPart);
        if (!TryComp<SurgeryBodyPartComponent>(partUid, out var partComp))
            return false;

        entity = (partUid.Value, partComp);
        return true;
    }
}

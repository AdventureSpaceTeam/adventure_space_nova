using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Server._Adventure.Medical.Surgery.Tools.Components;
using Content.Shared._Adventure.Medical.Surgery.Events;
using Content.Shared._Adventure.Medical.Surgery.Events.Organs;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Hands.Components;

namespace Content.Server._Adventure.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    public void ApplyToolOnOrgan(EntityUid user, EntityUid target, EntityUid? handEntity, OrganSlot slot)
    {
        if (handEntity is not { } tool || !HasComp<SurgeryToolComponent>(tool))
        {
            RemoveOrganAttachment(user, target, slot);
            return;
        }

        var model = new SurgeryOrganModel(
            User: GetNetEntity(user),
            Target: GetNetEntity(target),
            Tool: GetNetEntity(tool),
            Slot: slot
        );

        var surgeryEvent = new SurgeryToolApplyOrganEvent(model);
        RaiseLocalEvent(tool, surgeryEvent);
    }

    private void RemoveOrganAttachment(EntityUid user, EntityUid target, OrganSlot slot)
    {
        var attach = GetEntity(slot.Attachment);
        if (!TryComp<SurgeryToolComponent>(attach, out var attachComp))
            return;

        var removeToolFromOrgan = new SurgeryRemoveToolOrganDoAfter(slot);
        StartDoAfter(removeToolFromOrgan, user, target, (attach.Value, attachComp));
    }

    private void OnRemoveToolFromOrgan(EntityUid uid,
        SurgeryToolComponent component,
        SurgeryRemoveToolOrganDoAfter args)
    {
        if (args.Slot.Attachment == null)
            return;

        var slot = args.Slot;
        if (!TryComp<BodyPartComponent>(GetEntity(slot.Parent), out var part))
            return;

        part.Organs[slot.Id].Attachment = null;
        RemoveToolFrom(args.Slot.Parent, uid);
        _handsSystem.PickupOrDrop(args.User, uid);
        UpdateTarget(args.Target, SurgeryToolUsage.None);
    }

    private void AttachToolToOrganSlot(EntityUid tool, EntityUid user, OrganSlot slot)
    {
        if (!TryComp<HandsComponent>(user, out var userHands))
            return;

        if (userHands.ActiveHand?.HeldEntity is null ||
            !_handsSystem.TryDrop(user, userHands.ActiveHand, handsComp: userHands))
            return;

        if (!TryComp<BodyPartComponent>(GetEntity(slot.Parent), out var part))
            return;

        part.Organs[slot.Id].Attachment ??= GetNetEntity(tool);
        InsertToolTo(slot.Parent, tool);
    }
}

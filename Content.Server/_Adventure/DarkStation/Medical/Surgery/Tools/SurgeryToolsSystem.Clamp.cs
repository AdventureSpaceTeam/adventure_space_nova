using Content.Server.AdventureSpace.Medical.Surgery.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Events;
using Content.Server.AdventureSpace.Medical.Surgery.Tools.Components;
using Content.Shared.AdventureSpace.Medical.Surgery.Events.BodyParts;

namespace Content.Server.AdventureSpace.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeClamp()
    {
        SubscribeLocalEvent<SurgeryLargeClampComponent, SurgeryToolApplyEvent>(OnClampApply);
        SubscribeLocalEvent<SurgeryLargeClampComponent, SurgeryBodyPartDoAfter>(OnClampBodyPart);
    }

    private void OnClampBodyPart(EntityUid uid, SurgeryLargeClampComponent component, SurgeryBodyPartDoAfter args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var slot = args.Model.Slot;
        var bodyPartUid = GetEntity(slot.BodyPart);
        if (bodyPartUid != null)
            EnsureComp<SurgeryClampedComponent>(bodyPartUid.Value);

        AttachToolToPartSlot(uid, args.User, slot);
        PlaySoundWithPopup(args.User, uid, "surgery-large-clamp-attached");
        UpdateTarget(args.Target, SurgeryToolUsage.LargeClamp);
    }

    private void OnClampApply(EntityUid uid, SurgeryLargeClampComponent component, SurgeryToolApplyEvent args)
    {
        if (args.Handled)
            return;

        var tool = GetEntity(args.Model.Tool);
        if (!HasComp<SurgeryLargeClampComponent>(tool))
            return;

        var slot = args.Model.Slot;
        if (slot.Attachment != null)
        {
            var user = GetEntity(args.Model.User);
            _popupSystem.PopupEntity(Loc.GetString("surgery-part-already-has-attachment"), user, user);
            return;
        }

        var @event = new SurgeryBodyPartDoAfter(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }
}

using Content.Server.AdventureSpace.Medical.Surgery.Events;
using Content.Server.AdventureSpace.Medical.Surgery.Tools.Components;
using Content.Shared.AdventureSpace.Medical.Surgery.Events.Organs;

namespace Content.Server.AdventureSpace.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeSmallClamp()
    {
        SubscribeLocalEvent<SurgerySmallClampComponent, SurgeryOrganDoAfter>(OnSmallClamp);
        SubscribeLocalEvent<SurgerySmallClampComponent, SurgeryToolApplyOrganEvent>(OnSmallClampApply);
    }

    private void OnSmallClamp(EntityUid uid, SurgerySmallClampComponent component, SurgeryOrganDoAfter args)
    {
        if (args.Handled || args.Cancelled)
            return;

        AttachToolToOrganSlot(uid, args.User, args.Model.Slot);

        PlaySoundWithPopup(args.User, uid, "surgery-small-clamp-attached");
        UpdateTarget(args.Target, SurgeryToolUsage.SmallClamp);
    }

    private void OnSmallClampApply(EntityUid uid, SurgerySmallClampComponent component, SurgeryToolApplyOrganEvent args)
    {
        if (args.Handled)
            return;

        var slot = args.Model.Slot;
        if (slot.Attachment != null)
        {
            var user = GetEntity(args.Model.User);
            _popupSystem.PopupEntity(Loc.GetString("surgery-organ-already-has-attachment"), user, user);
            return;
        }

        var @event = new SurgeryOrganDoAfter(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }
}

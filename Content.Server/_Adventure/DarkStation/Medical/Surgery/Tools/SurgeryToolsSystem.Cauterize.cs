using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Server._Adventure.Medical.Surgery.Tools.Components;
using Content.Shared._Adventure.Medical.Surgery.Events.BodyParts;
using Content.Shared._Adventure.Medical.Surgery.Events.Organs;
using Content.Shared.Body.Part;

namespace Content.Server._Adventure.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeCauterize()
    {
        SubscribeLocalEvent<SurgeryCauterizerComponent, SurgeryToolApplyEvent>(OnCauterizeApply);
        SubscribeLocalEvent<SurgeryCauterizerComponent, SurgeryToolApplyOrganEvent>(OnCauterizeApplyOrgan);

        SubscribeLocalEvent<SurgeryCauterizerComponent, SurgeryOrganDoAfter>(OnCauterizeOrgan);
        SubscribeLocalEvent<SurgeryCauterizerComponent, SurgeryBodyPartDoAfter>(OnCauterizeBodyPart);
    }

    private void OnCauterizeBodyPart(EntityUid uid, SurgeryCauterizerComponent component, SurgeryBodyPartDoAfter args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var slot = args.Model.Slot;
        if (!TryComp<BodyPartComponent>(GetEntity(slot.Parent), out var part))
            return;

        part.Childs[slot.Id].Cauterised = true;
        PlaySoundWithPopup(args.User, uid, "surgery-wound-cauterised");
        UpdateTarget(args.Target, SurgeryToolUsage.Cauterizer);
    }

    private void OnCauterizeOrgan(EntityUid uid, SurgeryCauterizerComponent component, SurgeryOrganDoAfter args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var slot = args.Model.Slot;
        if (!TryComp<BodyPartComponent>(GetEntity(slot.Parent), out var part))
            return;

        part.Organs[slot.Id].Cauterised = true;
        PlaySoundWithPopup(args.User, uid, "surgery-wound-cauterised");
        UpdateTarget(args.Target, SurgeryToolUsage.Cauterizer);
    }

    private void OnCauterizeApplyOrgan(EntityUid uid,
        SurgeryCauterizerComponent component,
        SurgeryToolApplyOrganEvent args)
    {
        if (args.Handled)
            return;

        var slot = args.Model.Slot;
        if (slot.Child is not null)
            return;

        if (slot.Cauterised)
        {
            var user = GetEntity(args.Model.User);
            _popupSystem.PopupEntity(Loc.GetString("surgery-slot-already-cauterised"), user, user);
            return;
        }

        var @event = new SurgeryOrganDoAfter(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }

    private void OnCauterizeApply(EntityUid uid, SurgeryCauterizerComponent component, SurgeryToolApplyEvent args)
    {
        if (args.Handled)
            return;

        var slot = args.Model.Slot;
        if (slot.BodyPart != null)
            return;

        if (slot.Cauterised)
        {
            var user = GetEntity(args.Model.User);
            _popupSystem.PopupEntity(Loc.GetString("surgery-slot-already-cauterised"), user, user);
            return;
        }

        var @event = new SurgeryBodyPartDoAfter(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }
}

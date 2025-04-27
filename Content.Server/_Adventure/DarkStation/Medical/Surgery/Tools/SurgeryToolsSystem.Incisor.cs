using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Server._Adventure.Medical.Surgery.Tools.Components;
using Content.Shared._Adventure.Medical.Surgery.Events.BodyParts;
using Content.Shared._Adventure.Medical.Surgery.Events.Organs;
using Content.Shared.Body.Organ;
using Content.Shared.Hands.Components;

namespace Content.Server._Adventure.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeIncisor()
    {
        SubscribeLocalEvent<SurgeryIncisorComponent, SurgeryToolApplyEvent>(OnIncisorApply);
        SubscribeLocalEvent<SurgeryIncisorComponent, SurgeryToolApplyOrganEvent>(OnIncisorApplyOrgan);

        SubscribeLocalEvent<SurgeryIncisorComponent, SurgeryRemoveOrgan>(OnIncisorOrgan);
        SubscribeLocalEvent<SurgeryIncisorComponent, SurgeryBodyPartDoAfter>(OnIncisorBodyPart);
    }

    private void OnIncisorOrgan(EntityUid uid, SurgeryIncisorComponent component, SurgeryRemoveOrgan args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var organ = GetEntity(args.Model.Slot.Child);
        if (!TryComp<OrganComponent>(organ, out var organComp))
            return;

        if (!_bodySystem.RemoveOrgan(organ.Value, organComp))
            return;

        _handsSystem.PickupOrDrop(args.User, organ.Value);

        PlaySoundWithPopup(args.User, uid, "surgery-organ-removed");
        UpdateTarget(args.Target, SurgeryToolUsage.Incisor);
    }

    private void OnIncisorApplyOrgan(EntityUid uid, SurgeryIncisorComponent component, SurgeryToolApplyOrganEvent args)
    {
        if (args.Handled)
            return;

        var user = GetEntity(args.Model.User);
        if (!TryComp<HandsComponent>(user, out var hands))
            return;

        foreach (var hand in hands.Hands.Values)
        {
            if (hand.HeldEntity == hands.ActiveHandEntity)
                continue;

            if (!HasComp<SurgeryManipulatorComponent>(hand.HeldEntity))
                continue;

            var @event = new SurgeryRemoveOrgan(args.Model);
            StartDoAfter(@event, args.Model);

            args.Handled = true;
            return;
        }

        _popupSystem.PopupEntity("Возьми во вторую руку ретрактор или гемостат", user, user);
    }

    private void OnIncisorBodyPart(EntityUid uid, SurgeryIncisorComponent component, SurgeryBodyPartDoAfter args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        partEntity.Comp.State.Incised = true;

        PlaySoundWithPopup(args.User, uid, "surgery-incision-made");
        UpdatePartStatus(partEntity);
        UpdateTarget(args.Target, SurgeryToolUsage.Incisor);
    }

    private void OnIncisorApply(EntityUid uid, SurgeryIncisorComponent component, SurgeryToolApplyEvent args)
    {
        if (args.Handled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        if (partEntity.Comp.State is { Incisable: false } or { Incised: true })
            return;

        if (IsExoBodyPart(partEntity))
        {
            var user = GetEntity(args.Model.User);
            _popupSystem.PopupEntity(Loc.GetString("surgery-exo-skeleton-blocking"), user, user);

            return;
        }

        var @event = new SurgeryBodyPartDoAfter(args.Model);
        StartDoAfter(@event, args.Model);

        args.Handled = true;
    }
}

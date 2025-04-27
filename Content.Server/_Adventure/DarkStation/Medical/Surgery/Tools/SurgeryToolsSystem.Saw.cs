using Content.Server.AdventureSpace.Medical.Surgery.Events;
using Content.Server.AdventureSpace.Medical.Surgery.Tools.Components;
using Content.Shared.AdventureSpace.Medical.Surgery.Components;
using Content.Shared.AdventureSpace.Medical.Surgery.Events.BodyParts;
using Content.Shared.Body.Part;

namespace Content.Server.AdventureSpace.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem
{
    private void InitializeSaw()
    {
        SubscribeLocalEvent<SurgerySawComponent, SurgeryToolApplyEvent>(OnSawApply);

        SubscribeLocalEvent<SurgerySawComponent, SurgerySawExoBody>(OnSawExoBody);
        SubscribeLocalEvent<SurgerySawComponent, SurgerySawEndoBody>(OnSawEndoBody);
        SubscribeLocalEvent<SurgerySawComponent, SurgerySawRemoveBodyPart>(OnSawRemoveBodyPart);
    }

    private void OnSawRemoveBodyPart(EntityUid uid, SurgerySawComponent component, SurgerySawRemoveBodyPart args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var partUid = GetEntity(args.Model.Slot.BodyPart);
        if (!TryComp<BodyPartComponent>(partUid, out var partComp))
            return;

        _bodySystem.RequestRemovePart((partUid.Value, partComp));
        _handsSystem.PickupOrDrop(args.User, partUid.Value);

        PlaySoundWithPopup(args.User, uid, "surgery-body-part-removed");
        UpdateTarget(args.Target, SurgeryToolUsage.Saw);
    }

    private void OnSawEndoBody(EntityUid uid, SurgerySawComponent component, SurgerySawEndoBody args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        partEntity.Comp.State.EndoOpened = true;

        PlaySoundWithPopup(args.User, uid, "surgery-endoskeleton-opened");
        UpdatePartStatus(partEntity);
        UpdateTarget(args.Target, SurgeryToolUsage.Saw);
    }

    private void OnSawExoBody(EntityUid uid, SurgerySawComponent component, SurgerySawExoBody args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        partEntity.Comp.State.ExoOpened = true;

        PlaySoundWithPopup(args.User, uid, "surgery-exoskeleton-opened");
        UpdatePartStatus(partEntity);
        UpdateTarget(args.Target, SurgeryToolUsage.Saw);
    }

    private void OnSawApply(EntityUid uid, SurgerySawComponent component, SurgeryToolApplyEvent args)
    {
        if (args.Handled)
            return;

        if (!TryGetBodyPart(args.Model, out var part) || part is not { } partEntity)
            return;

        if (IsExoBodyPart(partEntity))
        {
            var @event = new SurgerySawExoBody(args.Model);
            StartDoAfter(@event, args.Model);
            args.Handled = true;

            return;
        }

        if (IsGoingRemoveBodyPart(partEntity))
        {
            var @event = new SurgerySawRemoveBodyPart(args.Model);
            StartDoAfter(@event, args.Model);
            args.Handled = true;

            return;
        }

        var state = partEntity.Comp.State;
        if (state is { Incisable: true, Incised: false } || !state.EndoSkeleton || state.EndoOpened)
            return;

        if (IsEndoBodyPart(partEntity))
        {
            var @event = new SurgerySawEndoBody(args.Model);
            StartDoAfter(@event, args.Model);
            args.Handled = true;

            return;
        }

        var user = GetEntity(args.Model.User);
        _popupSystem.PopupEntity(Loc.GetString("surgery-no-retractor"), user, user);
    }

    private bool IsExoBodyPart(Entity<SurgeryBodyPartComponent> entity)
    {
        return entity.Comp.State is { ExoSkeleton: true, ExoOpened: false };
    }

    private bool IsEndoBodyPart(Entity<SurgeryBodyPartComponent> entity)
    {
        var state = entity.Comp.State;
        if ((!state.Incisable || state.Incised) && state is { EndoSkeleton: true, EndoOpened: false })
            return !state.Incisable || BodyPartHasAttachment<SurgeryRetractorComponent>(entity);

        return false;
    }

    private bool IsGoingRemoveBodyPart(Entity<SurgeryBodyPartComponent> entity)
    {
        var state = entity.Comp.State;

        if (!TryComp<BodyPartComponent>(entity, out var bodyPart))
            return false;

        return !state.Incised && (!state.ExoSkeleton || state.ExoOpened) && bodyPart.BodyPartSlot is not null;
        // && bodyPart.BodyPartSlot is not null && !bodyPart.BodyPartSlot.IsRoot;
    }
}

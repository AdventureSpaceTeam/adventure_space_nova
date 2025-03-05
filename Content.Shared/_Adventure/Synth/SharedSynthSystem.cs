using Content.Shared.Containers.ItemSlots;
using Content.Shared.IdentityManagement;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.PowerCell.Components;
using Content.Shared._Adventure.Synth.Components;
using Content.Shared.UserInterface;
using Robust.Shared.Containers;

namespace Content.Shared._Adventure.Synth;

public abstract partial class SharedSynthSystem : EntitySystem
{
    [Dependency] protected readonly SharedContainerSystem Container = default!;
    [Dependency] protected readonly ItemSlotsSystem ItemSlots = default!;
    [Dependency] protected readonly ItemToggleSystem Toggle = default!;
    [Dependency] protected readonly SharedPopupSystem Popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SynthComponent, ItemSlotInsertAttemptEvent>(OnItemSlotInsertAttempt);
        SubscribeLocalEvent<SynthComponent, ItemSlotEjectAttemptEvent>(OnItemSlotEjectAttempt);
        SubscribeLocalEvent<SynthComponent, EntInsertedIntoContainerMessage>(OnInserted);
        SubscribeLocalEvent<SynthComponent, EntRemovedFromContainerMessage>(OnRemoved);
        SubscribeLocalEvent<SynthComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeedModifiers);

    }

    private void OnItemSlotInsertAttempt(EntityUid uid, SynthComponent component, ref ItemSlotInsertAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<PowerCellSlotComponent>(uid, out var cellSlotComp)) 
            return;

        if (!ItemSlots.TryGetSlot(uid, cellSlotComp.CellSlotId, out var cellSlot) || cellSlot != args.Slot)
            return;
    }

    private void OnItemSlotEjectAttempt(EntityUid uid, SynthComponent component, ref ItemSlotEjectAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<PowerCellSlotComponent>(uid, out var cellSlotComp))
            return;

        if (!ItemSlots.TryGetSlot(uid, cellSlotComp.CellSlotId, out var cellSlot) || cellSlot != args.Slot)
            return;
    }

    protected virtual void OnInserted(EntityUid uid, SynthComponent component, EntInsertedIntoContainerMessage args)
    {

    }

    protected virtual void OnRemoved(EntityUid uid, SynthComponent component, EntRemovedFromContainerMessage args)
    {

    }

    private void OnRefreshMovementSpeedModifiers(EntityUid uid, SynthComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        if (Toggle.IsActivated(uid))
            return;

        if (!TryComp<MovementSpeedModifierComponent>(uid, out var movement))
            return;

        var sprintDif = movement.BaseWalkSpeed / movement.BaseSprintSpeed;
        args.ModifySpeed(1f, sprintDif);
    }
}

using Content.Server.AdventureSpace.Medical.Surgery.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Events;
using Content.Server.AdventureSpace.Medical.Surgery.Tools;
using Content.Server.AdventureSpace.Medical.Surgery.Tools.Components;
using Content.Shared.AdventureSpace.Medical.Surgery;
using Content.Shared.AdventureSpace.Medical.Surgery.Components;
using Content.Shared.Body.Part;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;

namespace Content.Server.AdventureSpace.Medical.Surgery;

public sealed partial class SurgerySystem
{
    [Dependency] private readonly SurgeryToolsSystem _toolsSystem = default!;

    private void InitBUI()
    {
        SubscribeLocalEvent<SurgeryComponent, SurgerySlotButtonPressed>(OnSurgeryButtonPressed);
        SubscribeLocalEvent<SurgeryComponent, OrganSlotButtonPressed>(OnOrganButtonPressed);
        SubscribeLocalEvent<SurgeryComponent, SurgeryBodyUpdated>((_, _, args) => UpdateUiState(args.Body));
    }

    private void OnSurgeryButtonPressed(EntityUid uid, SurgeryComponent component, SurgerySlotButtonPressed args)
    {
        if (!TryComp<HandsComponent>(args.Actor, out var userHands))
            return;

        if (CheckBlockingInventory(uid, component))
        {
            _popupSystem.PopupEntity(Loc.GetString("surgery-blocked"), args.Actor, args.Actor);
            return;
        }

        if (HasComp<SurgeryToolComponent>(userHands.ActiveHandEntity))
        {
            _toolsSystem.ApplyToolOnBodyPartSlot(args.Actor, uid, userHands.ActiveHandEntity.Value, args.Slot);
            return;
        }

        _toolsSystem.ApplyEmptyHandsOnBodyPart(args.Actor, uid, args.Slot);
    }

    private void OnOrganButtonPressed(EntityUid uid, SurgeryComponent component, OrganSlotButtonPressed args)
    {
        if (!TryComp<HandsComponent>(args.Actor, out var userHands))
            return;

        if (CheckBlockingInventory(uid, component))
        {
            _popupSystem.PopupEntity(Loc.GetString("surgery-blocked"), args.Actor, args.Actor);
            return;
        }

        _toolsSystem.ApplyToolOnOrgan(args.Actor, uid, userHands.ActiveHandEntity, args.Slot);
    }

    public void UpdateUiState(EntityUid uid)
    {
        var bodyPartSlots = GetAllBodyPartSlots(uid);
        var organSlots = GetOpenPartOrganSlots(bodyPartSlots);

        var slotParts = new Dictionary<NetEntity, SharedPartStatus>();
        foreach (var slot in bodyPartSlots)
        {
            if (slot.BodyPart is null)
                continue;

            var part = GetEntity(slot.BodyPart.Value);

            if (!TryComp<SurgeryBodyPartComponent>(part, out var surgeryPart))
                continue;

            if (!TryComp<BodyPartComponent>(part, out var bodyPart))
                continue;

            var retracted = _toolsSystem.BodyPartHasAttachment<SurgeryRetractorComponent>(surgeryPart);
            var state = surgeryPart.State;

            slotParts.Add(
                slot.BodyPart.Value,
                new SharedPartStatus(
                    partType: bodyPart.PartType,
                    retracted: retracted,
                    incised: state.Incised,
                    opened: state.Opened,
                    endoOpened: state.EndoOpened,
                    exoOpened: state.ExoOpened
                )
            );
        }

        var uiState = new SurgeryBoundUserInterfaceState(bodyPartSlots, organSlots, slotParts);
        _userInterfaceSystem.SetUiState(uid, SurgeryUiKey.Key, uiState);
    }

    private bool CheckBlockingInventory(EntityUid uid, SurgeryComponent component)
    {
        if (!TryComp(uid, out InventoryComponent? inv) ||
            !_prototype.TryIndex<InventoryTemplatePrototype>(inv.TemplateId, out var prototype))
            return false;

        foreach (var slotDef in prototype.Slots)
        {
            if (!component.BlockingSlots.Contains(slotDef.Name))
                continue;

            if (!_inventory.TryGetSlotEntity(uid, slotDef.Name, out var item))
                continue;

            if (!HasComp<SurgeryGownComponent>(item.Value))
                return true;
        }

        return false;
    }
}

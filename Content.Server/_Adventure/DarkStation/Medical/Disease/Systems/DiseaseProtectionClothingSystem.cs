using Content.Server._Adventure.Medical.Disease.Components;
using Content.Server._Adventure.Medical.Disease.Data;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.Random;

namespace Content.Server._Adventure.Medical.Disease.Systems;

public sealed class DiseaseProtectionClothingSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventorySystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DiseaseProtectionClothingComponent, GotEquippedEvent>(OnGotEquipped);
        SubscribeLocalEvent<DiseaseProtectionClothingComponent, GotUnequippedEvent>(OnGotUnEquipped);
    }

    private void OnGotEquipped(Entity<DiseaseProtectionClothingComponent> ent, ref GotEquippedEvent args)
    {
        if (!IsOnTargetSlot(ent, args.Slot))
            return;

        var userComp = EnsureComp<DiseaseProtectionComponent>(args.Equipee);
        foreach (var (key, value) in ent.Comp.DiseaseProtection)
        {
            if (userComp.ProtectedSpreads.TryGetValue(key, out _))
            {
                var newValue = userComp.ProtectedSpreads[key] += value;
                if (newValue > 1f)
                    newValue = 1f;

                userComp.ProtectedSpreads[key] = newValue;
                continue;
            }

            userComp.ProtectedSpreads[key] = value;
        }
    }

    private void OnGotUnEquipped(Entity<DiseaseProtectionClothingComponent> ent, ref GotUnequippedEvent args)
    {
        if (!IsOnTargetSlot(ent, args.Slot))
            return;

        var userComp = EnsureComp<DiseaseProtectionComponent>(args.Equipee);
        foreach (var (key, value) in ent.Comp.DiseaseProtection)
        {
            if (!UserHasSameProtection((args.Equipee, userComp), key))
            {
                userComp.ProtectedSpreads.Remove(key);
                continue;
            }

            if (!userComp.ProtectedSpreads.ContainsKey(key))
                continue;

            var newValue = userComp.ProtectedSpreads[key] - value;
            if (newValue <= 0)
                userComp.ProtectedSpreads.Remove(key);
            else
                userComp.ProtectedSpreads[key] = newValue;
        }
    }

    private bool IsOnTargetSlot(EntityUid clothing, string slot)
    {
        if (!TryComp<ClothingComponent>(clothing, out var clothingComponent))
            return false;

        return clothingComponent.InSlot == slot;
    }

    private bool UserHasSameProtection(Entity<DiseaseProtectionComponent> ent, DiseaseSpreading spreading)
    {
        if (!TryComp<InventoryComponent>(ent, out var inventory))
            return false;

        var slotEnumerator = _inventorySystem.GetSlotEnumerator((ent, inventory));
        while (slotEnumerator.MoveNext(out var slot))
        {
            if (slot.ContainedEntity is not { } slotEnt)
                continue;

            if (!TryComp<DiseaseProtectionClothingComponent>(slotEnt, out var slotEntClothing))
                continue;

            if (slotEntClothing.DiseaseProtection.ContainsKey(spreading))
                return true;
        }

        return false;
    }

    public bool IsClothingProtectFromSpreading(EntityUid uid, DiseaseSpreading spreading)
    {
        if (!TryComp<DiseaseProtectionComponent>(uid, out var diseaseProtection))
            return false;

        if (!diseaseProtection.ProtectedSpreads.TryGetValue(spreading, out var protectValue))
            return false;

        return !_random.Prob(protectValue);
    }
}

using Content.Server.AdventureSpace.DarkForces.Vampire.Components;
using Content.Shared.AdventureSpace.Vampire.Attempt;
using Content.Shared.Inventory;

namespace Content.Server.AdventureSpace.GameRules.Vampire.Hunter;

public sealed class VampireImmnutiySystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InventoryComponent, VampireParalizeAttemptEvent>(OnInventoryParalizeAttempt);
        SubscribeLocalEvent<InventoryComponent, VampireHypnosisAttemptEvent>(OnInventoryHypnosisAttempt);

        SubscribeLocalEvent<VampireParalizeImmunityComponent, VampireParalizeAttemptEvent>(OnParalizeAttempt);
        SubscribeLocalEvent<VampireHypnosisImmunityComponent, VampireHypnosisAttemptEvent>(OnHypnosisEvent);
    }

    private void OnInventoryHypnosisAttempt(EntityUid uid,
        InventoryComponent component,
        VampireHypnosisAttemptEvent args)
    {
        if (_inventory.TryGetSlotEntity(uid, "eyes", out var item, component))
            RaiseLocalEvent(item.Value, args, true);

        if (_inventory.TryGetSlotEntity(uid, "neck", out var neckItem, component))
            RaiseLocalEvent(neckItem.Value, args, true);
    }

    private void OnInventoryParalizeAttempt(EntityUid uid,
        InventoryComponent component,
        VampireParalizeAttemptEvent args)
    {
        if (_inventory.TryGetSlotEntity(uid, "outerClothing", out var item, component))
            RaiseLocalEvent(item.Value, args, true);

        if (_inventory.TryGetSlotEntity(uid, "neck", out var neckItem, component))
            RaiseLocalEvent(neckItem.Value, args, true);
    }

    private void OnHypnosisEvent(EntityUid uid,
        VampireHypnosisImmunityComponent component,
        VampireHypnosisAttemptEvent args)
    {
        args.Cancel();
    }

    private void OnParalizeAttempt(EntityUid uid,
        VampireParalizeImmunityComponent component,
        VampireParalizeAttemptEvent args)
    {
        args.Cancel();
    }
}

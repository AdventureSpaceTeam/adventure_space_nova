using Content.Server.RPSX.DarkForces.Saint.Items.Cross;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.DarkForces.Saint.Items.Cross;

public sealed partial class SaintCrossSystem
{
    private static readonly DamageModifierSet FelDamageModify = new()
    {
        Coefficients =
        {
            ["Fel"] = 0.5f
        }
    };

    private void InitializeDamage()
    {
        SubscribeLocalEvent<SaintCrossComponent, InventoryRelayedEvent<DamageModifyEvent>>(OnDamageModify);
    }

    private void OnDamageModify(EntityUid uid,
        SaintCrossComponent component,
        InventoryRelayedEvent<DamageModifyEvent> args)
    {
        args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, FelDamageModify);
    }
}

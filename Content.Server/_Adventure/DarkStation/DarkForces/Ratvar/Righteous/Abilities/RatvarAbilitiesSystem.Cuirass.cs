using Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities.Enchantment.Armor;
using Content.Shared.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities;
using Content.Shared.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities.Armor;
using Content.Shared.Damage;
using Content.Shared.Inventory;

namespace Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities;

public sealed partial class RatvarAbilitiesSystem
{
    private const int MaxAbsorbCount = 2;

    private void InitializeCuirass()
    {
        SubscribeLocalEvent<RatvarCuirassComponent, InventoryRelayedEvent<DamageModifyEvent>>(OnCuirassDamageModify);
        SubscribeLocalEvent<RatvarEnchantmentableComponent, RatvarCuirassAbsorbEvent>(OnAbsorbEvent);
    }

    private void OnAbsorbEvent(EntityUid uid, RatvarEnchantmentableComponent component, RatvarCuirassAbsorbEvent args)
    {
        component.IsEnchantmentActive = true;
        component.DisableAbilityTick = _gameTiming.CurTime + TimeSpan.FromSeconds(8);
    }

    private void OnCuirassDamageModify(EntityUid uid,
        RatvarCuirassComponent component,
        InventoryRelayedEvent<DamageModifyEvent> args)
    {
        if (component.IsAbsorb)
        {
            if (component.AbsorbCount >= MaxAbsorbCount)
                component.IsAbsorb = false;
        }

        if (component is { IsAbsorb: true, AbsorbCount: < MaxAbsorbCount })
        {
            args.Args.Damage *= 0;
            component.AbsorbCount++;
        }
    }
}

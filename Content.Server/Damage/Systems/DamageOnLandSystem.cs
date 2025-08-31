using Content.Shared._Adventure.Bartender.Systems; // Adventure
using Content.Server.Damage.Components;
using Content.Shared.Damage;
using Content.Shared.Nutrition.Components; // Adventure
using Content.Shared.Throwing;

namespace Content.Server.Damage.Systems
{
    /// <summary>
    /// Damages the thrown item when it lands.
    /// </summary>
    public sealed class DamageOnLandSystem : EntitySystem
    {
        [Dependency] private readonly DamageableSystem _damageableSystem = default!;
        [Dependency] private readonly SpillProofThrowerSystem _nonspillthrower = default!; // Adventure

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<DamageOnLandComponent, LandEvent>(DamageOnLand);
        }

        private void DamageOnLand(EntityUid uid, DamageOnLandComponent component, ref LandEvent args)
        {
            // Adventure start
        if (args.User is { } user
            && _nonspillthrower.GetSpillProofThrow(user)
            && (HasComp<DrinkComponent>(uid)
            || (TryComp<EdibleComponent>(uid, out var edible) && edible.Solution == "drink")))
        {
            return;
        }
            // Adventure end
            _damageableSystem.TryChangeDamage(uid, component.Damage, component.IgnoreResistances);
        }
    }
}

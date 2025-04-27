using Content.Shared.Damage;
using Content.Shared.FixedPoint;

namespace Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities.Enchantment.Weapons;

[RegisterComponent]
public sealed partial class RatvarSwordComponent : Component
{
    [DataField]
    public DamageSpecifier SwordsmanDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            { "Piercing", -13 },
        },
    };
}

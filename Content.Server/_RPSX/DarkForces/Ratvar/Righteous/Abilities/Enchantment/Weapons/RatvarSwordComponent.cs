using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities.Enchantment.Weapons;

[RegisterComponent]
public sealed partial class RatvarSwordComponent : Component
{
    [DataField]
    public DamageSpecifier SwordsmanDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            {"Piercing", 13}
        }
    };
}

using System;
using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;

[RegisterComponent]
public sealed partial class VampireRegenComponent : Component
{
    [DataField("healingDamage")]
    public DamageSpecifier HealingDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            {"Blunt", -3},
            {"Slash", -3},
            {"Piercing", -3},
            {"Heat", -3},
            {"Cold", -3},
            {"Shock", -3},
            {"Holiness", -3}
        }
    };

    [DataField("nextTick", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextTick;

    [DataField("RegenTimes")]
    public int RegenTimes;
}

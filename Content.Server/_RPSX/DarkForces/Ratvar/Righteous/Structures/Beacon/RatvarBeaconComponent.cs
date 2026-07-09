using System;
using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Structures.Beacon;

[RegisterComponent]
public sealed partial class RatvarBeaconComponent : Component
{
    [DataField]
    public bool Enabled = true;

    [DataField]
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
            {"Burn", -3}
        }
    };

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan LastHealTick;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan LastPowerTick;

    [DataField]
    public int PowerPerTick = 2;
}

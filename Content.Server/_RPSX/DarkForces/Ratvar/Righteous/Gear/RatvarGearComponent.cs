using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Gear;

[RegisterComponent]
public sealed partial class RatvarGearComponent : Component
{
    [DataField]
    public bool Active;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan NextTick = TimeSpan.Zero;

    [DataField]
    public int Power;

    [DataField]
    public int PowerPerTick = 2;
}

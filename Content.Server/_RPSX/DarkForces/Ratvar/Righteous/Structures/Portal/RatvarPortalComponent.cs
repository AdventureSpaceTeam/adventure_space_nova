using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Structures.Portal;

[RegisterComponent]
public sealed partial class RatvarPortalComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan RatvarSpawnTick;
}

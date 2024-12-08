using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Ratvar.Righteous.Structures.Portal;

[RegisterComponent]
public sealed partial class RatvarPortalComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan RatvarSpawnTick;
}

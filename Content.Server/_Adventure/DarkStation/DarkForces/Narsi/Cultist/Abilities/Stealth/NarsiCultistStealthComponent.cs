using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Cultist.Abilities.Stealth;

[RegisterComponent]
public sealed partial class NarsiCultistStealthComponent : Component
{
    [DataField("tickToRemove", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickToRemove = TimeSpan.Zero;
}

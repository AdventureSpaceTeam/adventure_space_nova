using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Adventure.DarkForces.Narsi.Cultist.Abilities.Stealth;

[RegisterComponent]
public sealed partial class NarsiCultistStealthComponent : Component
{
    [DataField("tickToRemove", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickToRemove = TimeSpan.Zero;
}

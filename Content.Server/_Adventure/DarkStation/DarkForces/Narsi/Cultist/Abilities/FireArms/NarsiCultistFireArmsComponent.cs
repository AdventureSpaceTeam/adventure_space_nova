using Content.Shared.Damage;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Cultist.Abilities.FireArms;

[RegisterComponent]
public sealed partial class NarsiCultistFireArmsComponent : Component
{
    [DataField("canFireTargets")]
    public bool CanFireTargets;

    [DataField("additionDamage")]
    public DamageSpecifier DamageSpecifier = new();

    [DataField("tickToRemove", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickToRemove = TimeSpan.Zero;
}

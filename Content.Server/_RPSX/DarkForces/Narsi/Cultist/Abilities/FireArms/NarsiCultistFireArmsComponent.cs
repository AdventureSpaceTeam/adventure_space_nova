using System;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.RPSX.DarkForces.Narsi.Cultist.Abilities.FireArms;

[RegisterComponent]
public sealed partial class NarsiCultistFireArmsComponent : Component
{
    [DataField("tickToRemove", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickToRemove = TimeSpan.Zero;

    [DataField("additionDamage")]
    public DamageSpecifier DamageSpecifier = new();

    [DataField("canFireTargets")]
    public bool CanFireTargets;
}

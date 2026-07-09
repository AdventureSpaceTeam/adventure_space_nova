using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.RPSX.DarkForces.Desecrated.Pontific.Bonus;

[RegisterComponent]
public sealed partial class PontificBonusComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickToDelete = TimeSpan.Zero;

    [DataField]
    public float DamageMultiplier;

    [DataField]
    public float SpeedMultiplier;

    [DataField]
    public string Key = string.Empty;
}

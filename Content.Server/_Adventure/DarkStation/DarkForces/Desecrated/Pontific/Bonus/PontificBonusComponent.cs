using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.AdventureSpace.DarkForces.Desecrated.Pontific.Bonus;

[RegisterComponent]
public sealed partial class PontificBonusComponent : Component
{
    [DataField]
    public float DamageMultiplier;

    [DataField]
    public string Key = string.Empty;

    [DataField]
    public float SpeedMultiplier;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickToDelete = TimeSpan.Zero;
}

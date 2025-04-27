using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Adventure.DarkForces.Desecrated.Pontific.DarkAltar;

[RegisterComponent]
public sealed partial class PontificDarkAltarComponent : Component
{
    [DataField]
    public EntityUid? AltarOwner = EntityUid.Invalid;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public float FelRadius = 4.0f;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan TickIncreaseRadius = TimeSpan.Zero;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickProduceFel = TimeSpan.Zero;
}

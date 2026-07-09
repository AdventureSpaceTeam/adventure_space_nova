using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Desecrated.Pontific.DarkAltar;

[RegisterComponent]
public sealed partial class PontificDarkAltarComponent : Component
{
    [DataField]
    public EntityUid? AltarOwner = EntityUid.Invalid;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public float FelRadius = 4.0f;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickProduceFel = TimeSpan.Zero;

    [DataField(customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan TickIncreaseRadius = TimeSpan.Zero;
}

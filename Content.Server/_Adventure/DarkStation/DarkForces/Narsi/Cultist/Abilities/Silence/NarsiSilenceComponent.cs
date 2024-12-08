﻿using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Cultist.Abilities.Silence;

[RegisterComponent]
public sealed partial class NarsiSilenceComponent : Component
{
    [DataField("tickToRemove", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan TickToRemove = TimeSpan.Zero;
}

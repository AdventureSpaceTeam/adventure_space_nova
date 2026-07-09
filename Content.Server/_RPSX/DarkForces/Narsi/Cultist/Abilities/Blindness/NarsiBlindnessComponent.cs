using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Narsi.Cultist.Abilities.Blindness;

[RegisterComponent]
public sealed partial class NarsiBlindnessComponent : Component
{
    [DataField]
    public TimeSpan TimeToRemove = TimeSpan.Zero;
}

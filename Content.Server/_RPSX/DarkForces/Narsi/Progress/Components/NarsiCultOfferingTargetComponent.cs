using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Components;

[RegisterComponent]
public sealed partial class NarsiCultOfferingTargetComponent : Component
{
    [DataField]
    public List<EntityUid> Objectives = new();
}

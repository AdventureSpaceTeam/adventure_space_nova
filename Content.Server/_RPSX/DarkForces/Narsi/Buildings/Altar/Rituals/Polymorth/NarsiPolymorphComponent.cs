using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;

[RegisterComponent]
public sealed partial class NarsiPolymorphComponent : Component
{
    [DataField]
    public EntityUid AltarEntityUid;

    [DataField]
    public bool ReturnToAltar;
}

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities;

[RegisterComponent]
public sealed partial class RatvarAbilitiesComponent : Component
{
    [DataField]
    public EntProtoId ActionClockMagic = "RatvarClockMagic";

    [DataField]
    public EntityUid? ActionClockMagicEntity;

    [DataField]
    public EntProtoId ActionMidasTouch = "RatvarMidasTouch";

    [DataField]
    public EntityUid? ActionMidasTouchEntity;
}

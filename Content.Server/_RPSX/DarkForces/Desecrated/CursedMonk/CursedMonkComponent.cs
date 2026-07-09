using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.RPSX.DarkForces.Desecrated.CursedMonk;

[RegisterComponent]
public sealed partial class CursedMonkComponent : Component
{
    [DataField]
    public EntityUid? LightningActionEntity;

    [DataField]
    public EntProtoId LightingAction = "ActionAttackLighting";

    [DataField]
    public EntProtoId ZapBeamEntityId = "CursedMonkLightning";

}

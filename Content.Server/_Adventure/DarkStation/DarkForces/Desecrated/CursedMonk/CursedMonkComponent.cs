using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.DarkForces.Desecrated.CursedMonk;

[RegisterComponent]
public sealed partial class CursedMonkComponent : Component
{
    [DataField]
    public ProtoId<EntityPrototype> LightingAction = "ActionAttackLighting";

    [DataField]
    public EntityUid? LightningActionEntity;

    [DataField]
    public ProtoId<EntityPrototype> ZapBeamEntityId = "CursedMonkLightning";
}

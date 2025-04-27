using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.GameRules.SCP.Door;

[RegisterComponent]
public sealed partial class SCPDoorPryerComponent : Component
{
    [DataField]
    public EntProtoId PryerAction = "SCPOpenDoorAction";

    [DataField]
    public EntityUid? PryerActionUid;
}

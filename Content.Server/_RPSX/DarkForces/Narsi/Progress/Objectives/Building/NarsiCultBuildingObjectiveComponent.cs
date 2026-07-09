using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Building;

[RegisterComponent]
public sealed partial class NarsiCultBuildingObjectiveComponent : Component
{
    [DataField(required: true, serverOnly: true)]
    [ViewVariables(VVAccess.ReadOnly)]
    public NarsiBuilding BuildingType;
}

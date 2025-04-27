namespace Content.Server._Adventure.DarkForces.Narsi.Progress.Objectives.Building;

[RegisterComponent]
public sealed partial class NarsiCultBuildingObjectiveComponent : Component
{
    [DataField(required: true, serverOnly: true)]
    [ViewVariables(VVAccess.ReadOnly)]
    public NarsiBuilding BuildingType;
}

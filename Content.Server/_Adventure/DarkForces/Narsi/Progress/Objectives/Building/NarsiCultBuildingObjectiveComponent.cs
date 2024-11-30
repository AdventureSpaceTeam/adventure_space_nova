namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Progress.Objectives.Building;

[RegisterComponent]
public sealed partial class NarsiCultBuildingObjectiveComponent : Component
{
    [DataField(required: true, serverOnly: true)]
    [ViewVariables(VVAccess.ReadOnly)]
    public NarsiBuilding BuildingType;
}

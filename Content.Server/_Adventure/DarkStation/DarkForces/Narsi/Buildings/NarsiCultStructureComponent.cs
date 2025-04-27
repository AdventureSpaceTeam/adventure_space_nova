using Content.Server.AdventureSpace.DarkForces.Narsi.Progress.Objectives.Building;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Buildings;

[RegisterComponent]
public sealed partial class NarsiCultStructureComponent : Component
{
    [DataField(required: true)]
    public NarsiBuilding Building;
}

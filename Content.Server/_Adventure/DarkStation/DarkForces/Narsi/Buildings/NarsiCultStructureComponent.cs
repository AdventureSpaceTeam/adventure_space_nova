using Content.Server._Adventure.DarkForces.Narsi.Progress.Objectives.Building;

namespace Content.Server._Adventure.DarkForces.Narsi.Buildings;

[RegisterComponent]
public sealed partial class NarsiCultStructureComponent : Component
{
    [DataField(required: true)]
    public NarsiBuilding Building;
}

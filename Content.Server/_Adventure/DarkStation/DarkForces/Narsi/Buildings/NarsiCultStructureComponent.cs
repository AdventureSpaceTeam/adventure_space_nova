using Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Progress.Objectives.Building;

namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Buildings;

[RegisterComponent]
public sealed partial class NarsiCultStructureComponent : Component
{
    [DataField(required: true)]
    public NarsiBuilding Building;
}

using System.Linq;
using Content.Shared.RPSX.DarkForces.Narsi.Progress.Objectives;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Building;

public sealed class NarsiCultBuildingObjectiveSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NarsiBuildingEvent>(OnNarsiBuildingEvent);
    }

    private void OnNarsiBuildingEvent(NarsiBuildingEvent args)
    {
        var buildingType = args.Building;
        var query = EntityQuery<NarsiCultBuildingObjectiveComponent>().ToList();

        foreach (var component in query)
        {
            if (buildingType != component.BuildingType)
                continue;

            RaiseLocalEvent(new NarsiCultObjectiveCompleted(component.Owner));
        }
    }
}

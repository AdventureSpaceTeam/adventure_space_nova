using System.Linq;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Station.Components;
using Robust.Server.GameObjects;

namespace Content.Server.RPSX.Utils;

public static class StationUtils
{
    public static bool IsEntityOnMainStationOnly(EntityUid uid, IEntityManager entityManager)
    {
        if (!entityManager.TryGetComponent<TransformComponent>(uid, out var transform))
            return false;

        var stations = entityManager.GetAllComponents(typeof(StationEventEligibleComponent)).Select(x => x.Uid);

        foreach (var station in stations)
        {
            if (!entityManager.TryGetComponent<StationDataComponent>(station, out var dataComponent))
                continue;

            var grids = dataComponent.Grids.Where(grid => entityManager.HasComponent<BecomesStationComponent>(grid));
            if (grids.Any(grid => grid == transform.GridUid))
                return true;
        }

        return false;
    }

    public static EntityUid? GetStationByEntity(IEntityManager entityManager, EntityUid uid)
    {
        var stationSystem = entityManager.System<StationSystem>();
        var transformSystem = entityManager.System<TransformSystem>();
        var mapCoordinates = transformSystem.GetMapCoordinates(uid);

        return stationSystem.GetStationInMap(mapCoordinates.MapId);
    }
}

using System.Diagnostics.CodeAnalysis;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.RPSX.Utils;

public static class CoordinatesHelper
{
    public static bool TryGetEntityCoordinates(IEntityManager entityManager, EntityUid uid,
        [NotNullWhen(true)] out EntityCoordinates? coordinates)
    {
        coordinates = null;

        if (!entityManager.TryGetComponent<TransformComponent>(uid, out var transformComponent))
            return false;

        coordinates = transformComponent.Coordinates;

        return true;
    }

    public static MapCoordinates GetEntityMapPosition(IEntityManager entityManager, EntityUid uid)
    {
        var transformSystem = entityManager.System<TransformSystem>();
        return transformSystem.GetMapCoordinates(uid);
    }
}

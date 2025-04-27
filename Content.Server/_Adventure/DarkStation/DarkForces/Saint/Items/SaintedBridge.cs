using Content.Server._c4llv07e.Bridges;
using Content.Server._Adventure.DarkForces.Saint.Saintable;

namespace Content.Server._Adventure.DarkForces.Saint.Items;

public sealed class SaintedBridge : ISaintedBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public bool TryMakeSainted(EntityUid user, EntityUid uid)
    {
        return _entityManager.System<SaintedSystem>().TryMakeSainted(user, uid);
    }
}

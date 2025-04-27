using Content.Server._c4llv07e.Bridges;
using Content.Server.AdventureSpace.GameRules.Vampire.Role.Components;
using Content.Shared.AdventureSpace.DarkForces.Vampire.Components;
using Content.Shared.Mind;
using Content.Shared.Roles;

namespace Content.Server.AdventureSpace.GameRules.Vampire;

public sealed class VampireBridge : IVampireBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public void Initialize()
    {
    }

    public bool HasVampireRole(EntityUid mindId)
    {
        var entityManager = IoCManager.Resolve<IEntityManager>();
        var roleSystem = entityManager.System<SharedRoleSystem>();

        return roleSystem.MindHasRole<VampireRoleComponent>(mindId);
    }

    public bool IsVampire(EntityUid uid)
    {
        return _entityManager.HasComponent<VampireComponent>(uid);
    }

    public void MakeVampire(EntityUid mindId, MindComponent mind)
    {
        if (mind.OwnedEntity == null)
            return;

        // _entityManager.System<VampireRuleSystem>().MakeVampire(mind.OwnedEntity.Value);
    }
}

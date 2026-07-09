using Content.Shared.Mobs.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Systems;
using Content.Server.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.Helpers;
public sealed partial class MindHelpers : EntitySystem
{
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly MindSystem _mind = default!;

    public HashSet<EntityUid> GetAlivePlayers(EntityUid? exclude = null)
    {
        var allHumans = new HashSet<EntityUid>();
        var query = EntityQueryEnumerator<MobStateComponent, HumanoidAppearanceComponent>();
        while (query.MoveNext(out var uid, out var mobState, out _))
        {
            if (!_mind.TryGetMind(uid, out var mind, out var mindComp) || mind == exclude || !_mobState.IsAlive(uid, mobState))
                continue;
            allHumans.Add(uid);
        }
        return allHumans;
    }
}

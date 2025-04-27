using Content.Server._Adventure.GameRules.SCP.Station.Goals.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Adventure.GameRules.SCP.Station.Goals.SubSystems;

public sealed class SCPTransferSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCPTransferGoalComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(Entity<SCPTransferGoalComponent> ent, ref ComponentInit args)
    {
        ent.Comp.SpendTimeCheck = _timing.CurTime + ent.Comp.SpendTimeThreshold;
        ent.Comp.RequiredSecondsAtMainStation = _random.Next(1200, 3600);
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<SCPTransferGoalComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.SpendTimeCheck > _timing.CurTime)
                continue;

            component.SecondsAtMainStation += component.SpendTimeThreshold.TotalSeconds;

            if (component.SecondsAtMainStation < component.RequiredSecondsAtMainStation)
                continue;

            SCPGoalUtils.MakeTaskCompleted(uid, EntityManager);
        }
    }
}

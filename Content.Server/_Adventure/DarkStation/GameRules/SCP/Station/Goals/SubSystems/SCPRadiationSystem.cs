using Content.Server._Adventure.GameRules.SCP.Station.Goals.Components;
using Content.Shared.Radiation.Events;
using Robust.Shared.Random;

namespace Content.Server._Adventure.GameRules.SCP.Station.Goals.SubSystems;

public sealed class SCPRadiationSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCPRadiationGoalComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SCPRadiationGoalComponent, OnIrradiatedEvent>(OnIrradiated);
    }

    private void OnIrradiated(Entity<SCPRadiationGoalComponent> ent, ref OnIrradiatedEvent args)
    {
        if (!SCPGoalUtils.IsSCPOnMainStation(ent, EntityManager))
            return;

        ent.Comp.CurrentRadiationCount += args.TotalRads;

        if (ent.Comp.RequiredRadiationCount > ent.Comp.CurrentRadiationCount)
            return;

        SCPGoalUtils.MakeTaskCompleted(ent, EntityManager);
    }

    private void OnComponentInit(Entity<SCPRadiationGoalComponent> ent, ref ComponentInit args)
    {
        ent.Comp.RequiredRadiationCount = _random.Next(50, 100);
    }
}

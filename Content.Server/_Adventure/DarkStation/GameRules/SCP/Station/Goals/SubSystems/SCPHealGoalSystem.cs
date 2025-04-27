using Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Components;
using Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Events;
using Robust.Shared.Random;

namespace Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.SubSystems;

public sealed class SCPHealGoalSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCPHealGoalComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SCPHealGoalComponent, SCPHealEvent>(OnSCPHealEvent);
    }

    private void OnComponentInit(Entity<SCPHealGoalComponent> ent, ref ComponentInit args)
    {
        ent.Comp.RequirementCount = _random.Next(2, 4);
    }

    private void OnSCPHealEvent(Entity<SCPHealGoalComponent> ent, ref SCPHealEvent ev)
    {
        if (!SCPGoalUtils.IsSCPOnMainStation(ent, EntityManager))
            return;

        ent.Comp.CurrentCount += 1;

        if (ent.Comp.CurrentCount == ent.Comp.RequirementCount)
            SCPGoalUtils.MakeTaskCompleted(ent, EntityManager);
    }
}

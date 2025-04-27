using Content.Server.AdventureSpace.GameRules.SCP.Station.Components;
using Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Events;

namespace Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.SubSystems;

public static class SCPGoalUtils
{
    public static bool IsSCPOnMainStation(EntityUid uid, IEntityManager entityManager)
    {
        return entityManager.TryGetComponent<SCPGoalComponent>(uid, out var goalComponent) &&
               goalComponent.LocatedAtMainStation;
    }

    public static void MakeTaskCompleted(EntityUid uid, IEntityManager entityManager)
    {
        var ev = new SCPGoalComplete();
        entityManager.EventBus.RaiseLocalEvent(uid, ref ev);
    }
}

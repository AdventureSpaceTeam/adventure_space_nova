namespace Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Events;

public sealed class SCPHealEvent : EntityEventArgs
{
}

[ByRefEvent]
public record struct SCPGoalComplete;

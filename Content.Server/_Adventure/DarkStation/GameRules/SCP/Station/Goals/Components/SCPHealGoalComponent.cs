namespace Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Components;

[RegisterComponent]
public sealed partial class SCPHealGoalComponent : Component
{
    [DataField]
    public int CurrentCount;

    [DataField]
    public int RequirementCount = 2;
}

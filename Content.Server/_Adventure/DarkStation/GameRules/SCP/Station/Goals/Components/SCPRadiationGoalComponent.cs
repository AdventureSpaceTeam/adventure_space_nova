namespace Content.Server._Adventure.GameRules.SCP.Station.Goals.Components;

[RegisterComponent]
public sealed partial class SCPRadiationGoalComponent : Component
{
    [DataField]
    public float CurrentRadiationCount;

    [DataField]
    public float RequiredRadiationCount = 100;
}

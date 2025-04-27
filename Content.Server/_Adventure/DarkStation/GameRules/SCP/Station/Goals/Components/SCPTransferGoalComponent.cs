namespace Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Components;

[RegisterComponent]
public sealed partial class SCPTransferGoalComponent : Component
{
    [DataField]
    public double RequiredSecondsAtMainStation;

    [DataField]
    public double SecondsAtMainStation;

    [DataField]
    public TimeSpan SpendTimeCheck;

    [DataField]
    public TimeSpan SpendTimeThreshold = TimeSpan.FromSeconds(30);
}

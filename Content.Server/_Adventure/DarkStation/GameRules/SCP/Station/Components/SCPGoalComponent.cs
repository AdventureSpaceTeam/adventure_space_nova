namespace Content.Server._Adventure.GameRules.SCP.Station.Components;

[RegisterComponent]
public sealed partial class SCPGoalComponent : Component
{
    [DataField]
    public List<Type> Components = [];

    [DataField]
    public bool LocatedAtMainStation;
}

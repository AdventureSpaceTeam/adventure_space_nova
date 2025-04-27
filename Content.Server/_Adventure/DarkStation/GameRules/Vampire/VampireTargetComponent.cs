namespace Content.Server.AdventureSpace.GameRules.Vampire;

[RegisterComponent]
public sealed partial class VampireTargetComponent : Component
{
    [DataField("BloodDrinkedAmmount")]
    public int BloodDrinkedAmmount;
}

namespace Content.Server._Adventure.GameRules.Vampire;

[RegisterComponent]
public sealed partial class VampireTargetComponent : Component
{
    [DataField("BloodDrinkedAmmount")]
    public int BloodDrinkedAmmount;
}

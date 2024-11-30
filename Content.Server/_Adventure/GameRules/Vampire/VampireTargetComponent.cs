namespace Content.Server.AdventurePrivate._Alteros.GameRules.Vampire;

[RegisterComponent]
public sealed partial class VampireTargetComponent : Component
{
    [DataField("BloodDrinkedAmmount")]
    public int BloodDrinkedAmmount;
}

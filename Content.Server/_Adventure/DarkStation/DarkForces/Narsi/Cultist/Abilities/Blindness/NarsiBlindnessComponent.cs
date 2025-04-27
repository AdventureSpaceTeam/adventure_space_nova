namespace Content.Server._Adventure.DarkForces.Narsi.Cultist.Abilities.Blindness;

[RegisterComponent]
public sealed partial class NarsiBlindnessComponent : Component
{
    [DataField]
    public TimeSpan TimeToRemove = TimeSpan.Zero;
}

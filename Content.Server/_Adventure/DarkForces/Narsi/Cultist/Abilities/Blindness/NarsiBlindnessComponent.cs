namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Cultist.Abilities.Blindness;

[RegisterComponent]
public sealed partial class NarsiBlindnessComponent : Component
{
    [DataField]
    public TimeSpan TimeToRemove = TimeSpan.Zero;
}

namespace Content.Server._Adventure.Zombie.Hunter;

[RegisterComponent]
public sealed partial class ZombieHunterTargetComponent : Component
{
    [DataField]
    public EntityUid Hunter = EntityUid.Invalid;
}

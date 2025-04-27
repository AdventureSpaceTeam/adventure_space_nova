namespace Content.Server.AdventureSpace.Zombie.Hunter;

[RegisterComponent]
public sealed partial class ZombieHunterTargetComponent : Component
{
    [DataField]
    public EntityUid Hunter = EntityUid.Invalid;
}

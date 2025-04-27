using Content.Shared.AdventureSpace.Zombie.Hunter;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;

namespace Content.Server.AdventureSpace.Zombie.Hunter;

[RegisterComponent]
public sealed partial class ZombieHunterComponent : Component
{
    [DataField]
    public EntityUid? ActiveSoundEnt;

    [DataField]
    public SoundSpecifier AttackSound = new SoundPathSpecifier("/Audio/DarkStation/Mobs/Zombie/Hunter/attack.ogg");

    [DataField]
    public HunterAttackState AttackState = HunterAttackState.Idle;

    [DataField]
    public EntityUid CurrentTarget = EntityUid.Invalid;

    [DataField]
    public TimeSpan FallBackPeriod;

    [DataField]
    public SoundSpecifier FlySound = new SoundPathSpecifier("/Audio/DarkStation/Mobs/Zombie/Hunter/fly.ogg");

    [DataField]
    public TimeSpan HunterAttackPeriod;

    [DataField]
    public DamageSpecifier HunterDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            { "Blunt", 15 },
        },
    };

    [DataField]
    public TimeSpan HunterPreparePeriod;

    [DataField]
    public SoundSpecifier PrepareSound = new SoundPathSpecifier("/Audio/DarkStation/Mobs/Zombie/Hunter/alert.ogg");
}

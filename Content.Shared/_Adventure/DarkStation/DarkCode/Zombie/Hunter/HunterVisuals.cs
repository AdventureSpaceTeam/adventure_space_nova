using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Zombie.Hunter;

[Serializable, NetSerializable]
public enum HunterVisuals : byte
{
    State
}

[Serializable, NetSerializable]
public enum HunterAttackState : byte
{
    Idle,
    Prepare,
    Fly,
    Attack
}

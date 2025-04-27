using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Zombie.Smoker;

[Serializable, NetSerializable]
public sealed class ZombieSmokerMoveTargetRequestEvent : EntityEventArgs
{
    public bool Reeling;

    public ZombieSmokerMoveTargetRequestEvent(bool reeling)
    {
        Reeling = reeling;
    }
}

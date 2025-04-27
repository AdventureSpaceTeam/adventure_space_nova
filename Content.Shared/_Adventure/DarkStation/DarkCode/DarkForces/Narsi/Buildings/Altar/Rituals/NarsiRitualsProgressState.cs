using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.DarkForces.Narsi.Buildings.Altar.Rituals;

[Serializable, NetSerializable]
public enum NarsiRitualsProgressState
{
    Idle,
    Working,
    Delay
}

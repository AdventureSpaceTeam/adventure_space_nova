using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.DarkForces.Ratvar.Righteous.Abilities.Structures;

[Serializable, NetSerializable]
public enum RatvarAltarVisuals : byte
{
    State
}

[Serializable, NetSerializable]
public enum RatvarAltarState : byte
{
    UnAnchored,
    AnchoredIdle,
    Working
}

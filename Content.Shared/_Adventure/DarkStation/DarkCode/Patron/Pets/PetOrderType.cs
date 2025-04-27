using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Patron.Pets;

[Serializable, NetSerializable]
public enum PetOrderType : byte
{
    Stay,
    Follow,
    Attack
}

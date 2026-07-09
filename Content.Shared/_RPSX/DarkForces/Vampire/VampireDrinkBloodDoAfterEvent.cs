using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.Vampire;

[Serializable, NetSerializable]
public sealed partial class VampireDrinkBloodDoAfterEvent : DoAfterEvent
{
    public VampireDrinkBloodDoAfterEvent()
    {
    }
    public override DoAfterEvent Clone()
    {
        return this;
    }
}

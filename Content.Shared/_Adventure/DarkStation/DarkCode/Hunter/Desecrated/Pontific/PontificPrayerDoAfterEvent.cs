using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Hunter.Desecrated.Pontific;

[Serializable, NetSerializable]
public sealed partial class PontificPrayerDoAfterEvent : DoAfterEvent
{
    public override DoAfterEvent Clone()
    {
        return this;
    }
}

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Medical.Surgery.Events;

[Serializable, NetSerializable]
public sealed partial class SurgeryRemoveToolDoAfter : DoAfterEvent
{
    public NetEntity BodyPart;

    public SurgeryRemoveToolDoAfter(NetEntity bodyPart)
    {
        BodyPart = bodyPart;
    }

    public override DoAfterEvent Clone() => this;
}

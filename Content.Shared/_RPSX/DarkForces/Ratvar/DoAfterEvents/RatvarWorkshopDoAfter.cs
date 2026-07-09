using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.RPSX.DarkForces.Ratvar.DoAfterEvents;

[Serializable, NetSerializable]
public sealed partial class RatvarWorkshopDoAfter : DoAfterEvent
{
    [DataField]
    public EntProtoId EntityProduce;

    public override DoAfterEvent Clone()
    {
        return this;
    }
}

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Adventure.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryRejectionComponent : Component
{
    [DataField]
    public int RejectionCounter;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier RejectionDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            { "Cellular", 5 },
        },
    };

    [DataField]
    public int RejectionRounds = 3;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan RejectionThreshold = TimeSpan.FromSeconds(300);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan RejectionTick;
}

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Adventure.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryNecrosisComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    public readonly int NecrosisMaxTimes = 5;

    [DataField]
    [ViewVariables(VVAccess.ReadOnly)]
    public DamageSpecifier NecrosisDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            { "Cellular", 5 },
        },
    };

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NecrosisPeriodThreshold = TimeSpan.FromSeconds(30);

    /**
     * Necrosis Started
     */
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NecrosisPeriodTick;

    /**
     * Body Part Dead
     */
    [ViewVariables(VVAccess.ReadWrite)]
    public int NecrosisTimes;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Permanent;
}

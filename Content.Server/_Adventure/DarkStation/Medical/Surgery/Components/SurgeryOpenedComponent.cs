using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Adventure.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryOpenedComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadOnly)]
    public DamageSpecifier OpenedDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            { "Blunt", 5 },
        },
    };

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan OpenedThreshold = TimeSpan.FromSeconds(30);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan OpenedTick;
}

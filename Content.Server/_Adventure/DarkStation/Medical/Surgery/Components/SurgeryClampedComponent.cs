using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.AdventureSpace.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryClampedComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NecrosisStartTick;

    /**
     * To Start Necrosis
     */
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NecrosisThreshold = TimeSpan.FromSeconds(300);
}

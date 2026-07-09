using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.RPSX.DarkForces.Saint.Items.Cross;

[RegisterComponent]
public sealed partial class SaintCrossComponent : Component
{
    [DataField]
    public bool Sainted;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextTickToUpdate = TimeSpan.Zero;
}

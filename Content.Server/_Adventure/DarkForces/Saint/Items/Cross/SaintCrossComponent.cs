using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Saint.Items.Cross;

[RegisterComponent]
public sealed partial class SaintCrossComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextTickToUpdate = TimeSpan.Zero;

    [DataField]
    public bool Sainted;
}

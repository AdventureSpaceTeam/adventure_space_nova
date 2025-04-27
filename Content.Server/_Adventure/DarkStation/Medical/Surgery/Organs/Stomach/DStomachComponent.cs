using Content.Server.AdventureSpace.Medical.Surgery.Organs.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Organs.Components.Base;

namespace Content.Server.AdventureSpace.Medical.Surgery.Organs.Stomach;

[RegisterComponent]
public sealed partial class DStomachComponent : BaseRegeneratableOrganComponent, IIntervalOrgan
{
    [DataField]
    public List<string> ToxinsGroups = ["Poison"];

    [DataField]
    public TimeSpan NextCheckTime { get; set; }

    [DataField]
    public TimeSpan CheckInterval { get; set; }
}

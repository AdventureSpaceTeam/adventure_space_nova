using Content.Server.AdventureSpace.Medical.Surgery.Organs.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Organs.Components.Base;

namespace Content.Server.AdventureSpace.Medical.Surgery.Organs.Liver;

[RegisterComponent]
public sealed partial class ToxinFilterComponent : BaseRegeneratableOrganComponent, IIntervalOrgan
{
    [DataField]
    public List<string>
        FilterToxins = ["Poison", "Alcohol", "Narcotic"]; //these groups require special treatment RE liver

    [DataField]
    public float
        UnfilteredToxinRate = 0.1f; //if the body has no liver (or equiv), this is rate of additional toxins added

    [DataField]
    public string UnfilteredToxinReagent = "Toxin";

    [DataField]
    public TimeSpan NextCheckTime { get; set; }

    [DataField]
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(60);
}

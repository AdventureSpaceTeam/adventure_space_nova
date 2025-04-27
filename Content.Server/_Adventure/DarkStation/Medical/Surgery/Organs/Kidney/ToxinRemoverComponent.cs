using Content.Server.AdventureSpace.Medical.Surgery.Organs.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Organs.Components.Base;

namespace Content.Server.AdventureSpace.Medical.Surgery.Organs.Kidney;

[RegisterComponent]
public sealed partial class ToxinRemoverComponent : BaseRegeneratableOrganComponent, IIntervalOrgan
{
    public float
        BaseToxinRemovalRate = 0.25f; //if the body has no kidneys (or equiv), this is the rate of removal for toxins

    public float BuildUpRemovalMod = 0.01f;

    public List<string>
        RemoverToxins =
            ["Poison", "Alcohol", "Narcotic", "Medicine"]; //these groups require special treatment RE kidneys

    [DataField]
    public float ToxinRemovalRate = 1.0f;

    [DataField]
    public TimeSpan NextCheckTime { get; set; }

    [DataField]
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(60);
}

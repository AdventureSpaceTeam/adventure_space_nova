using Robust.Shared.Prototypes;

namespace Content.Shared._Adventure.Sponsors;

[Prototype]
public sealed partial class SponsorTierPrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; private set; } = default!;

    [DataField]
    public string? OocName { get; private set; } = null;

    [DataField]
    public Color? OocColor { get; private set; } = null;

    [DataField]
    public Color? GhostColor { get; private set; } = null;

    [DataField]
    public int AdditionalCharacterSlots { get; private set; } = 0;

    [DataField]
    public int Level { get; private set; } = 0;

    [DataField]
    public bool AllowRespawn { get; private set; } = false;
}

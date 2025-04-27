using Content.Shared.Whitelist;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals.Base;

[DataDefinition]
public sealed partial class NarsiRitualRequirements
{
    [DataField]
    public NarsiRitualBloodPuddleRequirements? BloodPuddleRequirements;

    [DataField]
    public EntityWhitelist? BuckedEntityWhitelist;

    [DataField(required: true)]
    public int CultistsCount;

    [DataField]
    public float EntitiesFoundingRange = 3f;

    [DataField]
    public List<NarsiRitualRequirementsEntity>? EntitiesRequirements;
}

[DataDefinition]
public sealed partial class NarsiRitualRequirementsEntity
{
    [DataField]
    public int Count;

    [DataField]
    public string Name = default!;

    [DataField(required: true)]
    public EntityWhitelist Whitelist = default!;
}

[DataDefinition]
public sealed partial class NarsiRitualBloodPuddleRequirements
{
    [DataField]
    public int Count;

    [DataField(required: true)]
    public List<string> ReagentsWhitelist = default!;
}

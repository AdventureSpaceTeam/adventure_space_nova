namespace Content.Server._Adventure.DarkForces.Narsi.Progress;

[RegisterComponent]
public sealed partial class NarsiCultProgressComponent : Component
{
    [DataField]
    public int BloodPoints;

    [DataField]
    public EntityUid? LeaderEntity;

    [DataField]
    public LeaderState LeaderState = LeaderState.Idle;

    [DataField(required: true)]
    public NarsiObjectivesData NarsiObjectives = default!;

    [DataField]
    public Dictionary<string, int> OpenedAbilities = new();
}

[DataDefinition]
public sealed partial class NarsiObjectivesData
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public bool CanSummonNarsi;

    [DataField]
    public int MaxKills;

    [DataField]
    public int MaxRituals;

    [DataField]
    public int MinKills;

    [DataField]
    public int MinRituals;

    [DataField]
    public TimeSpan NarsiObjectivesCheckThreshold = TimeSpan.FromSeconds(30);

    [DataField]
    public TimeSpan NarsiObjectivesCheckTime;

    [DataField]
    public EntityUid? NarsiSummonObjective;

    [DataField]
    public List<EntityUid> Objectives = new();
}

public enum LeaderState
{
    Idle,
    Dead,
    Selected,
    NoCandidates,
}

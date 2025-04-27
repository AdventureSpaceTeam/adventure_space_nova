using Content.Server.AdventureSpace.Medical.Disease.Components;

namespace Content.Server.AdventureSpace.Medical.Disease.Data;

[Serializable]
[DataDefinition]
public sealed partial class DiseaseData
{
    [DataField]
    public DiseaseCureData? Cure;

    [DataField]
    public TimeSpan ImmunityIncreaseThreshold;

    [DataField]
    public TimeSpan ImmunityIncreaseTime;

    [DataField]
    public ImmunityIncreaseType ImmunityIncreaseType = ImmunityIncreaseType.None;

    [DataField(required: true)]
    public List<DiseaseStage> Stages { get; set; } = default!;

    [DataField]
    public DiseaseStage? CurrentStage { get; set; }

    [DataField]
    public List<DiseaseSpreading>? Spreads { get; set; }

    //It's used for heal user by healing component

    #region HealingComponent

    [DataField]
    public TimeSpan CureTime;

    [DataField]
    public TimeSpan CureTimeThreshold = TimeSpan.FromSeconds(60);

    #endregion
}

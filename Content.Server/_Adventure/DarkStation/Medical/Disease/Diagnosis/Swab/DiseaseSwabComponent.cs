namespace Content.Server._Adventure.Medical.Disease.Diagnosis.Swab;

[RegisterComponent]
public sealed partial class DiseaseSwabComponent : Component
{
    [ViewVariables]
    public readonly List<string> DiseasesPrototypesIds = new();

    [DataField]
    public float SwabDelay = 4f;

    [DataField]
    public bool Used;
}

namespace Content.Server._Adventure.Medical.Disease.Diagnosis.Diagnoser.Components;

[RegisterComponent]
public sealed partial class DiseaseDiagnoserPaperComponent : Component
{
    [DataField]
    public List<string> DiseasesPrototypes = new();
}

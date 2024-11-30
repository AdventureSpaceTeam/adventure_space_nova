namespace Content.Server.AdventurePrivate._Alteros.Medical.Disease.Diagnosis.Diagnoser.Components;

[RegisterComponent]
public sealed partial class DiseaseDiagnoserPaperComponent : Component
{
    [DataField]
    public List<string> DiseasesPrototypes = new();
}

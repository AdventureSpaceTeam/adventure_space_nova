namespace Content.Server.AdventureSpace.Medical.Disease.Diagnosis.Diagnoser.Components;

[RegisterComponent]
public sealed partial class DiseaseDiagnoserComponent : Component
{
    [DataField]
    public TimeSpan DiagnoserFinishedThreshold = TimeSpan.FromSeconds(20);

    [DataField]
    public TimeSpan DiagnoserFinishedTime;

    [DataField]
    public DiseaseDiagnoserState State;

    [DataField]
    public string SwabSlotId = "Swab";
}

public enum DiseaseDiagnoserState : byte
{
    IDLE,
    RUNNING,
}

namespace Content.Server.AdventureSpace.Medical.Disease.Diagnosis.Vaccinator;

[RegisterComponent]
public sealed partial class DiseaseVaccinatorComponent : Component
{
    [DataField]
    public string PaperSlotId = "Paper";

    [DataField]
    public List<string> ReagentsSlotIds =
    [
        "ReagentOne",
        "ReagentTwo",
        "ReagentThree",
    ];

    [DataField]
    public List<string> RequiredReagents = [];

    [DataField]
    public string SelectedDisease = string.Empty;

    [DataField]
    public VaccinatorState State;

    [DataField]
    public TimeSpan VaccinatorFinishedThreshold = TimeSpan.FromSeconds(10);

    [DataField]
    public TimeSpan VaccinatorFinishedTime;
}

public enum VaccinatorState : byte
{
    IDLE,
    WAITING_DISEASE,
    WAITING_REAGENTS,
    RUNNING,
}

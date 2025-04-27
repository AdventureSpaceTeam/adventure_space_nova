using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.GameRules.SCP.Station.Goals.Prototypes;

[Prototype("SCPStationGoal")]
public sealed class SCPStationGoalPrototype : IPrototype
{
    [DataField]
    public ComponentRegistry Components = [];

    [DataField(required: true)]
    public string Message = default!;

    [DataField("SCPPrototypes")]
    public List<string>? Prototypes;

    [IdDataField]
    public string ID { get; private set; } = default!;
}

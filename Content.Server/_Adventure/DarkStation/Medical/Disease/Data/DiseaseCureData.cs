using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Medical.Disease.Data;

[Serializable]
[DataDefinition]
public sealed partial class DiseaseCureData
{
    [DataField]
    public List<string> ExternalReagents = [];

    [DataField]
    public bool Randomize;

    [DataField]
    public List<ProtoId<ReagentPrototype>> Reagents = [];

    [DataField]
    public List<string> VaccinatorReagents = [];
}

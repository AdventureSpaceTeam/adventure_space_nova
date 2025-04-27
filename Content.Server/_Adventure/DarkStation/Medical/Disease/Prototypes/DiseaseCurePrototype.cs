using Content.Server.AdventureSpace.Medical.Disease.Data;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Medical.Disease.Prototypes;

[Prototype("diseaseCure")]
public sealed class DiseaseCurePrototype : IPrototype
{
    [DataField]
    public List<string> ExternalReagents = [];

    [DataField]
    public bool Randomize;

    [DataField]
    public List<ProtoId<ReagentPrototype>> Reagents = [];

    [IdDataField]
    public string ID { get; set; } = default!;

    public DiseaseCureData CopyCure()
    {
        return new DiseaseCureData
        {
            Reagents = Reagents,
            ExternalReagents = ExternalReagents,
            Randomize = Randomize,
        };
    }
}

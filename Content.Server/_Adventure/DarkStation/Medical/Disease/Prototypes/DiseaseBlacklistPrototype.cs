using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Medical.Disease.Prototypes;

[Prototype("diseaseBlacklistPrototype")]
public sealed class DiseaseBlacklistPrototype : IPrototype
{
    [DataField]
    public List<string> BlackListReagents { get; set; } = default!;

    [IdDataField]
    public string ID { get; set; } = default!;
}

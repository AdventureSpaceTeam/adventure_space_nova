using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Roles.Salary;

[Serializable]
[Prototype("salaries")]
public sealed class SalariesPrototype : IPrototype
{
    [DataField]
    public Dictionary<ProtoId<JobPrototype>, int> Salaries = new();

    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;
}

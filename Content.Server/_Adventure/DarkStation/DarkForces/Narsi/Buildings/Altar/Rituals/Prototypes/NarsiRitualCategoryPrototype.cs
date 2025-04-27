using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals.Prototypes;

[Prototype]
public sealed class NarsiRitualCategoryPrototype : IPrototype
{
    [DataField(required: true, serverOnly: true)]
    public readonly string Name = default!;

    [DataField(required: true, serverOnly: true)]
    public List<ProtoId<NarsiRitualPrototype>> Rituals = new();

    [IdDataFieldAttribute]
    public string ID { get; } = default!;
}

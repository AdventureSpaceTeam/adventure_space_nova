using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Prototypes;

[Prototype, Serializable]
public sealed partial class NarsiRitualCategoryPrototype : IPrototype
{
    [IdDataFieldAttribute]
    public string ID { get; private set; } = default!;

    [DataField(required: true, serverOnly: true)]
    public string Name = default!;

    [DataField(required: true, serverOnly: true)]
    public List<ProtoId<NarsiRitualPrototype>> Rituals = new();
}

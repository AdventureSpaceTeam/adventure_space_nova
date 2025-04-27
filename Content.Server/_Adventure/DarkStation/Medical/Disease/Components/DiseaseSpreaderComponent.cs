using Content.Server._Adventure.Medical.Disease.Data;
using Content.Server._Adventure.Medical.Disease.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Medical.Disease.Components;

/**
 * Испольщуем для распространения болезней
 */
[RegisterComponent]
public sealed partial class DiseaseSpreaderComponent : Component
{
    [DataField]
    public Dictionary<DiseaseSpreading, List<ProtoId<DiseasePrototype>>> Diseases = new()
    {
        { DiseaseSpreading.Air, [] },
        { DiseaseSpreading.Contact, [] },
    };
}

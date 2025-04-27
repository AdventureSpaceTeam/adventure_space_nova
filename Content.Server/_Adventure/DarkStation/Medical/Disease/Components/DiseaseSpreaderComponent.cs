using Content.Server.AdventureSpace.Medical.Disease.Data;
using Content.Server.AdventureSpace.Medical.Disease.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Medical.Disease.Components;

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

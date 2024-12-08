using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Data;
using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Disease.Components;

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

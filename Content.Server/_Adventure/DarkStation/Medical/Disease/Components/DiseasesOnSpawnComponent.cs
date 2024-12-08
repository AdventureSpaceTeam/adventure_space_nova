using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseasesOnSpawnComponent : Component
{
    [DataField]
    public ProtoId<DiseasePrototype> DiseaseId;

    [DataField]
    public float Prob = -1;
}

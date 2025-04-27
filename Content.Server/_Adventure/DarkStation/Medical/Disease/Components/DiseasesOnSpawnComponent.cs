using Content.Server.AdventureSpace.Medical.Disease.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseasesOnSpawnComponent : Component
{
    [DataField]
    public ProtoId<DiseasePrototype> DiseaseId;

    [DataField]
    public float Prob = -1;
}

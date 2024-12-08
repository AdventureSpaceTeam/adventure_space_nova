using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Data;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Disease.Components;

[RegisterComponent]
public sealed partial class DiseaseProtectionComponent : Component
{
    [DataField]
    public Dictionary<DiseaseSpreading, float> ProtectedSpreads = [];
}

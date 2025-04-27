using Content.Shared._Adventure.DarkForces.Ratvar.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.DarkForces.Ratvar.Righteous.Abilities.Midas;

[RegisterComponent]
public sealed partial class MidasMaterialComponent : Component
{
    [DataField(required: true)]
    public List<ProtoId<RatvarMidasTouchablePrototype>> Targets = new();
}

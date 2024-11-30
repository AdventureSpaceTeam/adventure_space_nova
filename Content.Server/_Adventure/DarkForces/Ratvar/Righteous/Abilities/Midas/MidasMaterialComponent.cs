using Content.Shared.AdventureSpace.DarkForces.Ratvar.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Ratvar.Righteous.Abilities.Midas;

[RegisterComponent]
public sealed partial class MidasMaterialComponent : Component
{
    [DataField(required: true)]
    public List<ProtoId<RatvarMidasTouchablePrototype>> Targets = new();
}

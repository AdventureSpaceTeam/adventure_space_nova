using System.Collections.Generic;
using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Rituals;

[RegisterComponent]
public sealed partial class NarsiCultRitualObjectiveComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public NarsiRitualPrototype? RequiredRitual;

    [DataField(required:true)]
    [ViewVariables(VVAccess.ReadWrite)]
    public List<ProtoId<NarsiRitualPrototype>> Rituals;
}

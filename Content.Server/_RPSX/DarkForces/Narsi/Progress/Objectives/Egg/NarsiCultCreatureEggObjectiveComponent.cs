using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Egg;

[RegisterComponent]
public sealed partial class NarsiCultCreatureEggObjectiveComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<EntityPrototype>? CreatureId;

    [DataField(required: true)]
    [ViewVariables(VVAccess.ReadOnly)]
    public List<ProtoId<EntityPrototype>> AvailableCreatures;
}

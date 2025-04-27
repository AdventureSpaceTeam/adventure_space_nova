using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Progress.Objectives.Egg;

[RegisterComponent]
public sealed partial class NarsiCultCreatureEggObjectiveComponent : Component
{
    [DataField(required: true)]
    [ViewVariables(VVAccess.ReadOnly)]
    public List<ProtoId<EntityPrototype>> AvailableCreatures;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<EntityPrototype>? CreatureId;
}

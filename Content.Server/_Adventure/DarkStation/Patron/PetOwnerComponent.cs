using Content.Shared._Adventure.Patron.Pets;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Patron;

[RegisterComponent]
public sealed partial class PetOwnerComponent : Component
{
    [DataField]
    public Dictionary<EntProtoId, PetActionState> Actions = new();

    [DataField]
    public PetOrderType CurrentOrder = PetOrderType.Follow;

    [DataField]
    public bool IsSponsorPet;

    [DataField]
    public bool OnePetInGame;

    [DataField]
    public EntityUid? Pet;

    [DataField(required: true)]
    public EntProtoId PetId;

    [DataField]
    public string? PetName;
}

[ImplicitDataDefinitionForInheritors]
public sealed partial class PetActionState
{
    [DataField]
    public EntityUid? ActionUid;

    [DataField]
    public bool Available = true;
}

using System.Threading.Tasks;
using Content.Server._Adventure.Roles.Spawners;
using Content.Server.GameTicking;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Patron;

public sealed class PatronItemsSystem : EntitySystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetOrderFollow = "ActionPetOrderFollow";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetOrderStay = "ActionPetOrderStay";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetOrderAttack = "ActionPetOrderAttack";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetMakeGhostRole = "ActionPetMakeGhostRole";

    [Dependency] private readonly IPrototypeManager _protoMan = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete,
            after: new[] { typeof(AdditionalJobsSystem) });
    }

    private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
    {
        // if (!_sponsorsManager.TryGetSponsorTier(ev.Player.UserId, out var tier))
        //     return;
        //TODO BY UR

        // SpawnPet(tier, ev.Mob, ev.Profile);
    }

    private async Task SpawnPet( /*SponsorTier tier, TODO BY UR*/ EntityUid ent, HumanoidCharacterProfile profile)
    {
        // if (tier.PetCategories.Count == 0)
        //     return;

        var petData = profile.SponsorData.PetData;
        var petId = petData.PetId;
        var petName = petData.PetName;

        if (string.IsNullOrEmpty(petId) || !_protoMan.HasIndex(petId))
            return;

        var pet = new PetOwnerComponent
        {
            Owner = ent,
            PetId = petId,
            PetName = petName,
            IsSponsorPet = true,
            Actions = new Dictionary<EntProtoId, PetActionState>
            {
                { ActionPetOrderFollow, new PetActionState() },
                { ActionPetOrderStay, new PetActionState() },
                { ActionPetOrderAttack, new PetActionState() },
                { ActionPetMakeGhostRole, new PetActionState() },
            },
        };

        EntityManager.AddComponent(ent, pet);
    }
}

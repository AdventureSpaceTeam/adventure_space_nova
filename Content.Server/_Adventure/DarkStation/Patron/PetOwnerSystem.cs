using System.Linq;
using System.Numerics;
using Content.Server._Adventure.Roles.Spawners;
using Content.Server.GameTicking;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Mind;
using Content.Server.NPC;
using Content.Server.NPC.HTN;
using Content.Server.NPC.Systems;
using Content.Shared.Actions;
using Content.Shared._Adventure.Patron.Pets;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Pointing;
using Content.Shared.Weapons.Melee;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Patron;

public sealed class PatronPetsSystem : EntitySystem
{
    [ValidatePrototypeId<NpcFactionPrototype>]
    private const string Faction = "SimpleHostileNanoTrasen";

    [ValidatePrototypeId<HTNCompoundPrototype>]
    private const string RootTask = "PetCompound";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetOrderFollow = "ActionPetOrderFollow";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetOrderStay = "ActionPetOrderStay";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetOrderAttack = "ActionPetOrderAttack";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetMakeGhostRole = "ActionPetMakeGhostRole";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ActionPetRemoveGhostRole = "ActionPetRemoveGhostRole";

    private const int MaxPetHealth = 200;

    private const int MaxPetDamage = 5;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly HTNSystem _htn = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly MobThresholdSystem _mobThresholdSystem = default!;
    [Dependency] private readonly NPCSystem _npc = default!;
    [Dependency] private readonly NpcFactionSystem _npcFaction = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PetOwnerComponent, ComponentInit>(OnPetOwnerInit);
        SubscribeLocalEvent<PetOwnerComponent, PetOrderActionEvent>(OnPetOrderAction);
        SubscribeLocalEvent<PetOwnerComponent, PetMakeGhostRoleEvent>(OnMakeGhostRole);
        SubscribeLocalEvent<PetOwnerComponent, PetRemoveGhostRoleEvent>(OnRemoveGhostRole);
        SubscribeLocalEvent<PetOwnerComponent, AfterPointedAtEvent>(OnPetOwnerPointedAt);

        SubscribeLocalEvent<PetOwnerComponent, PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete,
            after: new[] { typeof(AdditionalJobsSystem) });
        SubscribeLocalEvent<PetComponent, ExaminedEvent>(OnPetExamined);
    }

    private void OnPlayerSpawnComplete(EntityUid uid, PetOwnerComponent component, PlayerSpawnCompleteEvent args)
    {
        if (component.Pet == null)
            return;

        var ownerTransform = Transform(uid);
        _transform.SetCoordinates(component.Pet.Value, ownerTransform.Coordinates);
    }

    private void OnPetExamined(EntityUid uid, PetComponent component, ExaminedEvent args)
    {
        if (component.PetOwner == null)
            return;

        args.PushMarkup("Личный питомец: [color=red]" + MetaData(component.PetOwner.Value).EntityName + "[/color]");
    }

    private void OnPetOwnerInit(EntityUid uid, PetOwnerComponent component, ComponentInit args)
    {
        if (!CanSpawnPet((uid, component)))
            return;

        foreach (var (actionId, state) in component.Actions)
        {
            if (!state.Available)
                continue;

            _actions.AddAction(uid, ref state.ActionUid, actionId);
        }

        UpdateActions(uid, component);

        var pet = Spawn(component.PetId, Transform(uid).Coordinates);
        SetupPet(pet, (uid, component));
    }

    private bool CanSpawnPet(Entity<PetOwnerComponent> owner)
    {
        if (!owner.Comp.OnePetInGame)
            return true;

        var query = EntityQueryEnumerator<PetOwnerComponent>();
        while (query.MoveNext(out _, out var component))
        {
            if (component.PetId != owner.Comp.PetId)
                continue;

            if (component.Pet != null)
                return false;
        }

        return true;
    }

    private void OnMakeGhostRole(EntityUid uid, PetOwnerComponent component, PetMakeGhostRoleEvent args)
    {
        var pet = component.Pet;
        if (pet == null || TryComp<MindContainerComponent>(pet, out var mind) && mind.HasMind)
        {
            SwitchToRemoveGhostRole((uid, component));
            return;
        }

        if (HasComp<GhostRoleComponent>(pet) || HasComp<GhostTakeoverAvailableComponent>(pet))
            return;

        var ghostRole = EnsureComp<GhostRoleComponent>(pet.Value);
        EnsureComp<GhostTakeoverAvailableComponent>(pet.Value);

        var ownerName = MetaData(uid).EntityName;
        var petName = MetaData(pet.Value).EntityName;

        ghostRole.RoleName = Loc.GetString("pet-owner-ghost-role-name", ("petName", petName), ("ownerName", ownerName));
        ghostRole.RoleDescription = Loc.GetString("pet-owner-ghost-role-description");
        ghostRole.RoleRules = Loc.GetString("pet-owner-ghost-role-rules");


        SwitchToRemoveGhostRole((uid, component));
    }

    private void SwitchToRemoveGhostRole(Entity<PetOwnerComponent> owner)
    {
        var removeGhostRole = owner.Comp.Actions[ActionPetRemoveGhostRole];
        var makeGhostRole = owner.Comp.Actions[ActionPetMakeGhostRole];

        _actions.RemoveAction(owner, makeGhostRole.ActionUid);
        _actions.AddAction(owner, ref removeGhostRole.ActionUid, ActionPetRemoveGhostRole);
    }

    private void SwitchToMakeGhostRole(Entity<PetOwnerComponent> owner)
    {
        var removeGhostRole = owner.Comp.Actions[ActionPetRemoveGhostRole];
        var makeGhostRole = owner.Comp.Actions[ActionPetMakeGhostRole];

        _actions.RemoveAction(owner, removeGhostRole.ActionUid);
        _actions.AddAction(owner, ref makeGhostRole.ActionUid, ActionPetMakeGhostRole);
    }

    private void OnRemoveGhostRole(EntityUid uid, PetOwnerComponent component, PetRemoveGhostRoleEvent args)
    {
        var pet = component.Pet;

        if (pet == null)
            return;

        if (_mindSystem.TryGetMind(pet.Value, out var mindId, out _))
            _gameTicker.OnGhostAttempt(mindId, false);

        RemComp<GhostRoleComponent>(pet.Value);
        RemComp<GhostTakeoverAvailableComponent>(pet.Value);

        SwitchToMakeGhostRole((uid, component));
    }

    private void SetupPet(EntityUid pet, Entity<PetOwnerComponent> owner)
    {
        var petComponent = EnsureComp<PetComponent>(pet);
        petComponent.PetOwner = owner;
        owner.Comp.Pet = pet;

        if (!string.IsNullOrEmpty(owner.Comp.PetName))
            _metaData.SetEntityName(pet, owner.Comp.PetName);

        SetupPetNpc(pet, owner);

        if (owner.Comp.IsSponsorPet)
        {
            SetupPetHealth(pet);
            SetupPetDamage(pet);
        }

        UpdatePet(pet, owner.Comp.CurrentOrder);
    }

    private void SetupPetNpc(EntityUid pet, EntityUid owner)
    {
        var htn = EnsureComp<HTNComponent>(pet);
        htn.RootTask = new HTNCompoundTask
        {
            Task = RootTask,
        };
        htn.Blackboard.SetValue("IdleRange", 3.5f);
        htn.Blackboard.SetValue("FollowCloseRange", 2.0f);
        htn.Blackboard.SetValue("FollowRange", 3.0f);

        _npc.SetBlackboard(pet, NPCBlackboard.FollowTarget, new EntityCoordinates(owner, Vector2.Zero));
        _npcFaction.AddFaction(pet, Faction);
    }

    private void SetupPetHealth(EntityUid pet)
    {
        if (!TryComp<MobThresholdsComponent>(pet, out var thresholds))
            return;

        var currentMaxHealth = thresholds.Thresholds.First(threshold => threshold.Value == MobState.Dead);
        if (currentMaxHealth.Key <= MaxPetHealth)
            return;

        if (_mobThresholdSystem.TryGetThresholdForState(pet, MobState.Alive, out _))
            _mobThresholdSystem.SetMobStateThreshold(pet, 0, MobState.Alive);

        if (_mobThresholdSystem.TryGetThresholdForState(pet, MobState.Critical, out _))
            _mobThresholdSystem.SetMobStateThreshold(pet, MaxPetHealth / 2, MobState.Critical);

        if (_mobThresholdSystem.TryGetThresholdForState(pet, MobState.Dead, out _))
            _mobThresholdSystem.SetMobStateThreshold(pet, MaxPetHealth, MobState.Dead);
    }

    private void SetupPetDamage(EntityUid pet)
    {
        if (!TryComp<MeleeWeaponComponent>(pet, out var weapon))
            return;

        var totalDamage = weapon.Damage.DamageDict.Sum(damage => damage.Value.Value);
        if (totalDamage <= MaxPetDamage)
            return;

        var damagePerGroup = MaxPetDamage / weapon.Damage.DamageDict.Count;
        var newDamageDict = new Dictionary<string, FixedPoint2>();
        foreach (var (key, value) in weapon.Damage.DamageDict)
        {
            newDamageDict[key] = damagePerGroup;
        }

        weapon.Damage.DamageDict = newDamageDict;
    }

    private void OnPetOwnerPointedAt(EntityUid uid, PetOwnerComponent component, AfterPointedAtEvent args)
    {
        if (component.CurrentOrder != PetOrderType.Attack || component.Pet == null)
            return;

        if (uid == args.Pointed)
            return;

        _npc.SetBlackboard(component.Pet.Value, NPCBlackboard.CurrentOrderedTarget, args.Pointed);
    }

    private void OnPetOrderAction(EntityUid uid, PetOwnerComponent component, PetOrderActionEvent args)
    {
        if (component.CurrentOrder == args.Type || component.Pet == null)
            return;

        component.CurrentOrder = args.Type;

        UpdateActions(uid, component);
        UpdatePet(component.Pet.Value, args.Type);

        args.Handled = true;
    }

    private void UpdateActions(EntityUid uid, PetOwnerComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        _actions.SetToggled(component.Actions[ActionPetOrderStay].ActionUid,
            component.CurrentOrder == PetOrderType.Stay);
        _actions.SetToggled(component.Actions[ActionPetOrderAttack].ActionUid,
            component.CurrentOrder == PetOrderType.Attack);
        _actions.SetToggled(component.Actions[ActionPetOrderFollow].ActionUid,
            component.CurrentOrder == PetOrderType.Follow);

        _actions.StartUseDelay(component.Actions[ActionPetOrderStay].ActionUid);
        _actions.StartUseDelay(component.Actions[ActionPetOrderAttack].ActionUid);
        _actions.StartUseDelay(component.Actions[ActionPetOrderFollow].ActionUid);

        if (component.CurrentOrder != PetOrderType.Attack && component.Pet != null)
            _npc.SetBlackboard(component.Pet.Value, NPCBlackboard.CurrentOrderedTarget, EntityUid.Invalid);
    }

    private void UpdatePet(EntityUid uid, PetOrderType orderType)
    {
        if (!TryComp<HTNComponent>(uid, out var htn))
            return;

        if (htn.Plan != null)
            _htn.ShutdownPlan(htn);

        _npc.SetBlackboard(uid, NPCBlackboard.CurrentOrders, orderType);
        _htn.Replan(htn);
    }
}

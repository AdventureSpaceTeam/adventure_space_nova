using System.Collections.Generic;
using Content.Server.RPSX.DarkForces.Vampire.EUI;
using Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;
using Content.Server.RPSX.DarkForces.Vampire.Role.Events;
using Content.Server.EUI;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Shared.Mind;
using Content.Shared.NPC.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.RPSX.DarkForces.Vampire;
using Content.Shared.RPSX.Vampire;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using VampireComponent = Content.Shared.RPSX.DarkForces.Vampire.Components.VampireComponent;

namespace Content.Server.RPSX.DarkForces.Vampire.Role;

public sealed partial class VampireSystem : EntitySystem
{
    [Dependency] private readonly NpcFactionSystem _factionSystem = default!;
    [Dependency] private readonly VampireAbilitiesSystem _vampireAbilities = default!;
    [Dependency] private readonly EuiManager _euiManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly ObjectivesSystem _objectives = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private const string BloodObjective = "VampireBloodObjective";

    [ValidatePrototypeId<EntityPrototype>]
    private const string EnthrallObjective = "VampireEnthrallObjective";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireComponent, ComponentInit>(OnVampireInit);
        SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);
        SubscribeLocalEvent<VampireComponent, VampireOpenHelpDialogEvent>(OnOpenHelpDialogEvent);
        SubscribeLocalEvent<VampireComponent, VampireAbilitySelectedEvent>(OnAbilitySelected);

        InitializeSaint();
    }

    private void OnVampireInit(EntityUid uid, VampireComponent component, ComponentInit args)
    {
        RemComp<HungerComponent>(uid);

        _factionSystem.RemoveFaction(uid, "NanoTrasen", false);
        _factionSystem.AddFaction(uid, "Vampire");
        _vampireAbilities.OnVampireInit(uid, component);

        component.NextObjectivesCheckTick = _gameTiming.CurTime + component.ObjectivesCheckPeriod;

        if (!_mindSystem.TryGetMind(uid, out var mindId, out var mind))
            return;

        CreateObjectives(uid, component, mindId, mind);
    }

    private void OnVampireShutdown(EntityUid uid, VampireComponent component, ComponentShutdown args)
    {
        _vampireAbilities.OnVampireShutdown(uid, component);
    }

    private void CreateObjectives(EntityUid uid, VampireComponent component, EntityUid mindId,
        MindComponent mindComponent)
    {
        var bloodObjective = _objectives.TryCreateObjective(mindId, mindComponent, BloodObjective);
        var enthrallObjective = _objectives.TryCreateObjective(mindId, mindComponent, EnthrallObjective);

        if (bloodObjective != null)
        {
            _mindSystem.AddObjective(mindId, mindComponent, bloodObjective.Value);
            component.Objectives.Add(bloodObjective.Value);
        }

        if (enthrallObjective != null)
        {
            _mindSystem.AddObjective(mindId, mindComponent, enthrallObjective.Value);
            component.Objectives.Add(enthrallObjective.Value);
        }
    }

    private void OnOpenHelpDialogEvent(EntityUid uid, VampireComponent component, VampireOpenHelpDialogEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<ActorComponent>(uid, out var actor))
            return;

        var session = actor.PlayerSession;

        if (component.AbilitiesUiOpen)
            return;

        var ui = new VampireAbilitiesEUI(uid);
        var state = new VampireAbilitiesState(
            GetNetEntity(uid),
            GetOpenedAbilities(uid, component),
            component.TotalDrunkBlood,
            component.CurrentBloodAmount
        );

        _euiManager.OpenEui(ui, session);
        ui.SendMessage(state);

        component.AbilitiesUiOpen = true;
        args.Handled = true;
    }

    private List<string> GetOpenedAbilities(EntityUid uid, VampireComponent component)
    {
        return _vampireAbilities.GetOpenedAbilities(uid, component);
    }

    private void OnAbilitySelected(EntityUid uid, VampireComponent component, VampireAbilitySelectedEvent args)
    {
        _vampireAbilities.TryOpenAbility(uid, component, args);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _gameTiming.CurTime;
        var query = EntityQueryEnumerator<VampireComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.FullPower || component.NextObjectivesCheckTick > curTime)
                continue;

            if (!IsObjectivesCompleted(uid, component))
            {
                component.NextObjectivesCheckTick = curTime + component.ObjectivesCheckPeriod;
                continue;
            }

            _vampireAbilities.AddFullPowerAction(uid, component);
        }
    }

    private bool IsObjectivesCompleted(EntityUid uid, VampireComponent component)
    {
        if (!_mindSystem.TryGetMind(uid, out var mindId, out _))
            return false;

        foreach (var objective in component.Objectives)
        {
            var objectiveInfo = _objectives.GetInfo(objective, mindId);
            if (objectiveInfo == null)
            {
                continue;
            }

            if (objectiveInfo.Value.Progress < 1f)
                return false;
        }

        return true;
    }
}

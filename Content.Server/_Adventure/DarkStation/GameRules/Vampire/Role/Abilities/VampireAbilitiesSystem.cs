using Content.Server._Adventure.GameRules.Vampire.Role.Events;
using Content.Server._Adventure.GameRules.Vampire.Role.Trall;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Server.Stunnable;
using Content.Shared.Actions;
using Content.Shared._Adventure.Vampire;
using Content.Shared.Damage;
using Content.Shared.Popups;
using Robust.Shared.Console;
using Robust.Shared.Timing;
using VampireComponent = Content.Shared._Adventure.DarkForces.Vampire.Components.VampireComponent;


namespace Content.Server._Adventure.GameRules.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly IConsoleHost _console = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookupSystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly StunSystem _stunSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly VampireTrallSystem _trallSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitChiropteamScreech();
        InitBats();
        InitDrinkBlood();
        InitFullPower();
        InitParalyze();
        InitRejuvenate();
        InitShapeshift();
        InitTrall();
        InitCharge();
    }

    public void OnVampireInit(EntityUid uid, VampireComponent component)
    {
        if (component.FullPower)
        {
            _console.ExecuteCommand($"scale {uid} 1,2");
            return;
        }

        _actionsSystem.AddAction(uid, ref component.ActionStatisticEntity, component.ActionStatistic);
        _actionsSystem.AddAction(uid, ref component.ActionDrinkBloodEntity, component.ActionDrinkBlood);
        _actionsSystem.AddAction(uid, ref component.ActionRejuvenateEntity, component.ActionRejuvenate);
        _actionsSystem.AddAction(uid, ref component.ActionFlashEntity, component.ActionFlash);
    }


    public void OnVampireShutdown(EntityUid uid, VampireComponent component)
    {
        _actionsSystem.RemoveAction(uid, component.ActionStatisticEntity);
        _actionsSystem.RemoveAction(uid, component.ActionDrinkBloodEntity);
        _actionsSystem.RemoveAction(uid, component.ActionRejuvenateEntity);
        _actionsSystem.RemoveAction(uid, component.ActionFullPowerEntity);
        _actionsSystem.RemoveAction(uid, component.ActionFlashEntity);
    }

    public void TryOpenAbility(EntityUid uid, VampireComponent component, VampireAbilitySelectedEvent args)
    {
        if (!component.FullPower && component.CurrentBloodAmount < args.BloodRequired)
            return;

        EntityUid? actionUid = null;

        if (!_actionsSystem.AddAction(uid, ref actionUid, out var actionComp, args.ActionId))
            return;

        actionComp.Keywords.Add(args.ActionId);

        component.OpenedAbilities[args.ActionId] = actionUid.Value;
        component.CurrentBloodAmount -= args.BloodRequired;

        if (args.ReplaceId == null)
            return;

        var actions = _actionsSystem.GetActions(uid);
        foreach (var action in actions)
        {
            var actionId = FindVampireAction(action.Comp);
            if (actionId != args.ReplaceId)
                continue;

            _actionsSystem.RemoveAction(uid, action.Id);
            component.OpenedAbilities.Remove(actionId);
        }
    }

    public void AddFullPowerAction(EntityUid uid, VampireComponent component)
    {
        _actionsSystem.AddAction(uid, ref component.ActionFullPowerEntity, component.ActionFullPower);
    }

    private bool CanUseAbility(VampireComponent component, IVampireEvent args)
    {
        if (component.FullPower || component.CurrentBloodAmount >= args.BloodCost)
            return true;

        _popupSystem.PopupEntity(Loc.GetString("vampire-not-enought-blood"),
            component.Owner,
            component.Owner,
            PopupType.Medium);
        return false;
    }

    public List<string> GetOpenedAbilities(EntityUid uid, VampireComponent component)
    {
        var openedAbilities = new List<string>();
        var actions = _actionsSystem.GetActions(uid);
        foreach (var action in actions)
        {
            var actionId = FindVampireAction(action.Comp);
            if (actionId == null)
                continue;

            openedAbilities.Add(actionId);
        }

        return openedAbilities;
    }

    private void OnActionUsed(EntityUid uid, VampireComponent component, IVampireEvent args)
    {
        component.CurrentBloodAmount -= args.BloodCost;

        if (component.CurrentBloodAmount < 0)
            component.CurrentBloodAmount = 0;
    }
}

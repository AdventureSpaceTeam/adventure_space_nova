using Content.Server.Mind;
using Content.Shared.Actions;
using Content.Shared._Adventure.Vampire;
using Robust.Shared.Prototypes;
using VampireComponent = Content.Shared._Adventure.DarkForces.Vampire.Components.VampireComponent;

namespace Content.Server._Adventure.GameRules.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string FullPowerMobVampire = "MobVampire";

    [Dependency] private readonly MindSystem _mindSystem = default!;

    private void InitFullPower()
    {
        SubscribeLocalEvent<VampireComponent, VampireFullPowerEvent>(OnVampireFullPowerEvent);
    }

    private void OnVampireFullPowerEvent(EntityUid uid, VampireComponent component, VampireFullPowerEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        if (!_mindSystem.TryGetMind(uid, out var mindId, out _))
            return;

        var coordinates = Transform(uid).Coordinates;
        var mobVampire = Spawn(FullPowerMobVampire, coordinates);

        _mindSystem.TransferTo(mindId, mobVampire);

        QueueDel(uid);
        TransferActions(uid, component, mobVampire);

        args.Handled = true;
    }

    private void TransferActions(EntityUid uid, VampireComponent oldComponent, EntityUid mobVampire)
    {
        var component = EnsureComp<VampireComponent>(mobVampire);
        var originalActions = _actionsSystem.GetActions(uid);
        foreach (var action in originalActions)
        {
            var keyWord = FindVampireAction(action.Comp);
            if (keyWord == null)
                continue;

            if (!oldComponent.OpenedAbilities.ContainsValue(action.Id))
                continue;

            EntityUid? newActionUid = null;
            if (!_actionsSystem.AddAction(mobVampire, ref newActionUid, keyWord))
                continue;

            component.OpenedAbilities[keyWord] = newActionUid.Value;
        }
    }

    private string? FindVampireAction(BaseActionComponent component)
    {
        foreach (var keyword in component.Keywords)
        {
            if (!keyword.Contains("Vampire"))
                continue;

            return keyword;
        }

        return null;
    }
}

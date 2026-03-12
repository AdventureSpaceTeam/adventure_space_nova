using System.Linq;
using Content.Server.RPSX.DarkForces.Vampire.Role.Components;
using Content.Server.Mind;
using Content.Shared.Actions;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.RPSX.Vampire;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Content.Shared.Actions.Components;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    [Dependency] private readonly MindSystem _mindSystem = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private const string FullPowerMobVampire = "MobVampire";

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
            var keyWord = FindVampireAction(action.Owner);
            if (keyWord == null)
                continue;

            if (!oldComponent.OpenedAbilities.ContainsValue(action.Owner))
                continue;

            EntityUid? newActionUid = null;
            if (!_actionsSystem.AddAction(mobVampire, ref newActionUid, keyWord))
                continue;

            component.OpenedAbilities[keyWord] = newActionUid.Value;
        }
    }

    private string? FindVampireAction(EntityUid uid)
    {
        if (_tagSystem.HasTag(uid, "VampireAbility"))
            return "VampireAbility";

        return null;
    }
}

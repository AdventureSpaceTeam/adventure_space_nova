using System;
using System.Collections.Generic;
using System.Linq;
using Content.Server.RPSX.DarkForces.Vampire.Role.Components;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.RPSX.Vampire;
using Content.Shared.RPSX.Vampire.Attempt;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Content.Shared.Body.Components;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    private const float HydrationFactor = 10.0f;
    private const int MaxBloodFromTarget = 200;
    private const int DrinkedBloodPerAction = 20;

    private static readonly HashSet<string> AvilableBloodReagents = new() { "Blood", "InsectBlood" };
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly ThirstSystem _thirstSystem = default!;

    private void InitDrinkBlood()
    {
        SubscribeLocalEvent<VampireComponent, VampireDrinkBloodAblityEvent>(OnVampireDrinkBlood);
        SubscribeLocalEvent<VampireComponent, VampireDrinkBloodDoAfterEvent>(OnVampireDrinkBloodDoAfterEvent);
    }

    private void OnVampireDrinkBlood(EntityUid uid, VampireComponent component, VampireDrinkBloodAblityEvent args)
    {
        if (args.Handled || args.Target == args.Performer || !CanUseAbility(component, args))
            return;

        if (!IsTargetHasBlood(args.Target))
        {
            _popupSystem.PopupEntity(Loc.GetString("vampire-not-correct-blood"), args.Target, uid);
            return;
        }

        var attemptEvent = new VampireDrinkBloodAttemptEvent();
        RaiseLocalEvent(args.Target, attemptEvent);

        if (attemptEvent.Cancelled)
        {
            _popupSystem.PopupEntity(Loc.GetString("vampire-target-defence"), uid, uid);
            return;
        }

        var vampireTargetComp = EnsureComp<VampireTargetComponent>(args.Target);
        if (vampireTargetComp.BloodDrinkedAmmount >= MaxBloodFromTarget)
        {
            _popupSystem.PopupEntity(Loc.GetString("vampire-target-max-blood"), args.Target, uid, PopupType.Large);
            return;
        }

        SendDrinkBloodDoAfter(args);
        args.Handled = true;
    }

    private bool IsTargetHasBlood(EntityUid target)
    {
        var isHumanoid = HasComp<HumanoidAppearanceComponent>(target);
        var hasCorrectBlood = TryComp(target, out BloodstreamComponent? bloodStream)
            && bloodStream.BloodReferenceSolution.Contents.Any(r => AvilableBloodReagents.Contains(r.Reagent.Prototype));

        return isHumanoid && hasCorrectBlood;
    }

    private void SendDrinkBloodDoAfter(VampireDrinkBloodAblityEvent args)
    {
        var doAfterEvent = new VampireDrinkBloodDoAfterEvent();
        var doAfterEventArgs = new DoAfterArgs(
            EntityManager,
            args.Performer,
            TimeSpan.FromSeconds(2),
            doAfterEvent,
            args.Performer,
            args.Target,
            args.Performer
        )
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            MovementThreshold = 1.0f
        };

        _doAfterSystem.TryStartDoAfter(doAfterEventArgs);
    }

    private void OnVampireDrinkBloodDoAfterEvent(EntityUid uid, VampireComponent component,
        VampireDrinkBloodDoAfterEvent args)
    {
        if (args.Handled)
            return;

        if (args.Cancelled)
        {
            return;
        }

        if (!TryComp<VampireTargetComponent>(args.Target, out var vampireTargetComp))
            return;

        _bloodstreamSystem.TryModifyBleedAmount((EntityUid) args.Target, -DrinkedBloodPerAction);
        vampireTargetComp.BloodDrinkedAmmount += DrinkedBloodPerAction;

        component.CurrentBloodAmount += DrinkedBloodPerAction;
        component.TotalDrunkBlood += DrinkedBloodPerAction;

        if (TryComp(uid, out ThirstComponent? thirst))
            _thirstSystem.ModifyThirst(uid, thirst, HydrationFactor);

        args.Handled = true;
    }
}

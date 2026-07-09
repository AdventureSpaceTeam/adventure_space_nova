using System;
using Content.Shared.Bed.Sleep;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.RPSX.Vampire;
using Content.Shared.RPSX.Vampire.Attempt;
using Robust.Shared.GameObjects;
using VampireParalizeAttemptEvent = Content.Shared.RPSX.Vampire.Attempt.VampireParalizeAttemptEvent;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    private void InitParalyze()
    {
        SubscribeLocalEvent<VampireComponent, VampireFlashEvent>(OnVampireFlashEvent);
        SubscribeLocalEvent<VampireComponent, VampireHypnosisEvent>(OnVampireHypnosisEvent);
        SubscribeLocalEvent<VampireComponent, VampireHypnoseDoAfterEvent>(OnVampireHypnosisDoAfterEvent);
    }

    private void OnVampireFlashEvent(EntityUid uid, VampireComponent component, VampireFlashEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        args.Handled = true;

        var coordinates = Transform(uid).Coordinates;
        var humansToStun = _entityLookupSystem.GetEntitiesInRange<HumanoidAppearanceComponent>(coordinates, 2f);

        foreach (var human in humansToStun)
        {
            var targetUid = human.Owner;
            if (targetUid == uid)
                continue;

            var attemptEvent = new VampireParalizeAttemptEvent(targetUid, uid, component.FullPower);
            RaiseLocalEvent(targetUid, attemptEvent, true);

            if (attemptEvent.Cancelled)
                continue;

            _stunSystem.TryAddParalyzeDuration(targetUid, TimeSpan.FromSeconds(5));
        }

        OnActionUsed(uid, component, args);
    }

    private void OnVampireHypnosisDoAfterEvent(EntityUid uid, VampireComponent component, VampireHypnoseDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Target == null)
            return;

        args.Handled = true;

        var target = (EntityUid) args.Target;
        var sleepComponent = new SleepingComponent
        {
            Owner = target,
            // CoolDownEnd = _timing.CurTime + TimeSpan.FromSeconds(30),
            Cooldown = _timing.CurTime + TimeSpan.FromSeconds(30)
        };

        if (HasComp<SleepingComponent>(target))
            RemComp<SleepingComponent>(target);

        EntityManager.AddComponent(target, sleepComponent);
    }

    private void OnVampireHypnosisEvent(EntityUid uid, VampireComponent component, VampireHypnosisEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args) || args.Performer == args.Target)
            return;

        var attemptEvent = new VampireHypnosisAttemptEvent(args.Target, uid, component.FullPower);
        RaiseLocalEvent(args.Target, attemptEvent, true);

        if (attemptEvent.Cancelled)
            return;

        args.Handled = true;

        SendHypnoseDoAfterEvent(uid, args);
        OnActionUsed(uid, component, args);
    }

    private void SendHypnoseDoAfterEvent(EntityUid uid, VampireHypnosisEvent args)
    {
        var doAfterEvent = new VampireHypnoseDoAfterEvent();
        var doAfterEventArgs = new DoAfterArgs(
            EntityManager,
            args.Performer,
            TimeSpan.FromSeconds(3),
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
}

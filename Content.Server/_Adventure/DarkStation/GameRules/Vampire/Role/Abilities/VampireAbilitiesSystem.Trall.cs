using Content.Shared.AdventureSpace.Vampire;
using Content.Shared.AdventureSpace.Vampire.Attempt;
using Content.Shared.DoAfter;
using VampireComponent = Content.Shared.AdventureSpace.DarkForces.Vampire.Components.VampireComponent;

namespace Content.Server.AdventureSpace.GameRules.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    private void InitTrall()
    {
        SubscribeLocalEvent<VampireComponent, VampireTrallDoAfterEvent>(OnVampireTrallDoAfterEvent);
        SubscribeLocalEvent<VampireComponent, VampireEnthrallEvent>(OnVampireEnthrallEvent);
    }

    private void OnVampireTrallDoAfterEvent(EntityUid uid, VampireComponent component, VampireTrallDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Target == null)
            return;

        _trallSystem.MakeTrall(uid, args.Target.Value);
        args.Handled = true;
    }

    private void OnVampireEnthrallEvent(EntityUid uid, VampireComponent component, VampireEnthrallEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        var attemptEvent = new VampireHypnosisAttemptEvent(uid, args.Target, component.FullPower);
        RaiseLocalEvent(args.Target, attemptEvent, true);

        if (attemptEvent.Cancelled || !_trallSystem.CanBeTrall(args.Target))
        {
            _popupSystem.PopupEntity(Loc.GetString("vampire-trall-not-valid"), uid, uid);
            return;
        }

        SendEnthrallDoAfterEvent(uid, component, args);
        args.Handled = true;
    }

    private void SendEnthrallDoAfterEvent(EntityUid uid, VampireComponent component, VampireEnthrallEvent args)
    {
        var doAfterEvent = new VampireTrallDoAfterEvent();
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
            MovementThreshold = 1.0f,
        };

        _doAfterSystem.TryStartDoAfter(doAfterEventArgs);
        OnActionUsed(uid, component, args);
    }
}

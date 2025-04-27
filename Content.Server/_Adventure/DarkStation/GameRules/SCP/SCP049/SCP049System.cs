using System.Linq;
using Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Events;
using Content.Server.Chat.Managers;
using Content.Server.DoAfter;
using Content.Server.Flash;
using Content.Server.Popups;
using Content.Server.Speech.Components;
using Content.Server.Stunnable;
using Content.Shared.Actions;
using Content.Shared.AdventureSpace.SCP.SCP_049;
using Content.Shared.DoAfter;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rejuvenate;
using Robust.Shared.Player;
using SCP049Component = Content.Shared.AdventureSpace.SCP.SCP_049.Components.SCP049Component;
using SCP049ThrallComponent = Content.Shared.AdventureSpace.SCP.SCP_049.Components.SCP049ThrallComponent;

namespace Content.Server.AdventureSpace.GameRules.SCP.SCP049;

public sealed class SCP049System : SharedSCP049System
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly StunSystem _stunSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCP049Component, ComponentInit>(OnPlagueDoctorInit);
        SubscribeLocalEvent<SCP049Component, ComponentRemove>(OnPlagueDoctorRemove);

        SubscribeLocalEvent<SCP049Component, SCP049HealEvent>(OnHealEvent);
        SubscribeLocalEvent<SCP049Component, SCP049ThrallEvent>(OnMakeThrallEvent);
        SubscribeLocalEvent<SCP049Component, SCP049UnThrallEvent>(OnUnThrallEvent);
        SubscribeLocalEvent<SCP049Component, SCP049FlashEvent>(OnFlashEvent);

        SubscribeLocalEvent<SCP049Component, SCP049HealDoAfterEvent>(OnHealDoAfterEvent);
        SubscribeLocalEvent<SCP049Component, SCP049ThrallDoAfterEvent>(OnMakeThrallDoAfterEvent);
        SubscribeLocalEvent<SCP049Component, SCP049UnThrallDoAfterEvent>(OnUnThrallDoAfterEvent);
    }

    private void OnPlagueDoctorInit(EntityUid uid, SCP049Component component, ComponentInit args)
    {
        _actionsSystem.AddAction(uid, ref component.HealActionUid, component.HealActionPrototype);
        _actionsSystem.AddAction(uid, ref component.ThrallActionUid, component.ThrallActionPrototype);
        _actionsSystem.AddAction(uid, ref component.UnThrallActionUid, component.UnThrallActionPrototype);
        _actionsSystem.AddAction(uid, ref component.FlashActionUid, component.FlashActionPrototype);
    }

    private void OnPlagueDoctorRemove(EntityUid uid, SCP049Component component, ComponentRemove args)
    {
        _actionsSystem.RemoveAction(uid, component.HealActionUid);
        _actionsSystem.RemoveAction(uid, component.ThrallActionUid);
        _actionsSystem.RemoveAction(uid, component.UnThrallActionUid);
        _actionsSystem.RemoveAction(uid, component.FlashActionUid);
    }

    private void OnHealDoAfterEvent(Entity<SCP049Component> ent, ref SCP049HealDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (args.Target is not { } target)
            return;

        RaiseLocalEvent(target, new RejuvenateEvent());
        RaiseLocalEvent(ent, new SCPHealEvent());
    }

    private void OnMakeThrallDoAfterEvent(Entity<SCP049Component> ent, ref SCP049ThrallDoAfterEvent args)
    {
        if (args.Target is not { } target)
            return;

        SetupThrall(ent, target);
        args.Handled = true;
    }

    private void SetupThrall(Entity<SCP049Component> ent, EntityUid target)
    {
        var thrallComp = EnsureComp<SCP049ThrallComponent>(target);
        if (!TryComp<ReplacementAccentComponent>(target, out var accent))
        {
            var replacementAccentComp = EnsureComp<ReplacementAccentComponent>(target);
            replacementAccentComp.Accent = ent.Comp.ThrallAccentProtoId;
        }
        else
        {
            thrallComp.OldAccent = accent.Accent;
            accent.Accent = ent.Comp.ThrallAccentProtoId;
        }

        _popupSystem.PopupEntity(
            Loc.GetString("scp-049-thrall-start"),
            target,
            target
        );

        SendTextToChat(target, Loc.GetString("scp-049-thrall-description"));
    }

    private void OnUnThrallDoAfterEvent(Entity<SCP049Component> ent, ref SCP049UnThrallDoAfterEvent args)
    {
        if (args.Target is not { } target)
            return;

        if (!TryComp<SCP049ThrallComponent>(target, out var thrallComponent))
            return;

        if (thrallComponent.OldAccent != null && TryComp<ReplacementAccentComponent>(target, out var accent))
            accent.Accent = thrallComponent.OldAccent;
        else
            RemComp<ReplacementAccentComponent>(target);

        RemComp<SCP049ThrallComponent>(target);

        SendTextToChat(target, Loc.GetString("scp-049-thrall-end"));
    }

    private void OnFlashEvent(Entity<SCP049Component> ent, ref SCP049FlashEvent args)
    {
        var target = args.Target;

        var attempt = new FlashAttemptEvent(target, ent, ent);
        RaiseLocalEvent(target, attempt, true);

        if (attempt.Cancelled)
        {
            args.Handled = true;
            return;
        }

        _stunSystem.TryParalyze(target, ent.Comp.StunTime, true);
        args.Handled = true;
    }

    private void OnHealEvent(Entity<SCP049Component> ent, ref SCP049HealEvent args)
    {
        var @event = new SCP049HealDoAfterEvent();
        StartDoAfter(@event, ent.Comp.HealDelay, ent, args.Target);

        args.Handled = true;
    }

    private void OnMakeThrallEvent(Entity<SCP049Component> ent, ref SCP049ThrallEvent args)
    {
        if (!CanMakeThrall(ent, args.Target))
            return;

        var @event = new SCP049ThrallDoAfterEvent();
        StartDoAfter(@event, ent.Comp.MakeThrallDelay, ent, args.Target);

        args.Handled = true;
    }

    private bool CanMakeThrall(Entity<SCP049Component> ent, EntityUid target)
    {
        var zombies = EntityQuery<SCP049ThrallComponent>();
        if (zombies.Count() >= 2)
        {
            _popupSystem.PopupEntity("Вы не можете иметь больше двух прислужников", ent);
            return false;
        }

        if (_mobStateSystem.IsDead(target))
        {
            _popupSystem.PopupEntity("Цель мертва...", ent);
            return false;
        }

        return true;
    }

    private void OnUnThrallEvent(Entity<SCP049Component> ent, ref SCP049UnThrallEvent args)
    {
        var @event = new SCP049UnThrallDoAfterEvent();
        StartDoAfter(@event, ent.Comp.UnThrallDelay, ent, args.Target);

        args.Handled = true;
    }

    private void StartDoAfter(DoAfterEvent @event, TimeSpan delay, EntityUid user, EntityUid target)
    {
        var doAfterEvent = new DoAfterArgs(
            EntityManager,
            user,
            delay,
            @event,
            user,
            target
        )
        {
            BreakOnDamage = false,
            BreakOnMove = true,
            MovementThreshold = 2f,
        };

        _doAfter.TryStartDoAfter(doAfterEvent);
    }

    private void SendTextToChat(EntityUid target, string message)
    {
        if (!TryComp<ActorComponent>(target, out var actor))
            return;

        _chatManager.DispatchServerMessage(
            actor.PlayerSession,
            message
        );
    }
}

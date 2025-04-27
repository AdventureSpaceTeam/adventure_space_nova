using Content.Shared._Adventure.SCP._173;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Components;
using Robust.Shared.Timing;

namespace Content.Client._Adventure.SCP;

public sealed class SCP173System : SharedSCP173System
{
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SCP173FreezeComponent, AttackAttemptEvent>(OnTryAttack);
        SubscribeLocalEvent<SCP173FreezeComponent, OnLookStateChangedEvent>(OnLookStateChanged);
    }

    private void OnLookStateChanged(EntityUid uid, SCP173FreezeComponent component, OnLookStateChangedEvent args)
    {
        if (!args.IsLookedAt)
            return;

        if (TryComp<InputMoverComponent>(uid, out var input))
            input.CanMove = false;
    }

    private void OnTryAttack(EntityUid uid, SCP173FreezeComponent component, AttackAttemptEvent args)
    {
        if (!_timing.IsFirstTimePredicted)
            return;

        var target = args.Target.GetValueOrDefault();
        if (!target.IsValid())
            return;

        if (!CanAttack(uid, target, component))
            args.Cancel();
    }
}

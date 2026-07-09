using Content.Server.Damage.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.RPSX.Vampire;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;

    private static TimeSpan RegenPeriod = TimeSpan.FromSeconds(2);

    private void InitRejuvenate()
    {
        SubscribeLocalEvent<VampireComponent, VampireRejuvenateEvent>(OnVampireRejuvenate);
        SubscribeLocalEvent<VampireComponent, VampireRejuvenatePlusEvent>(OnVampireRejuvenatePlusEvent);
    }

    private void OnVampireRejuvenatePlusEvent(EntityUid uid, VampireComponent component,
        VampireRejuvenatePlusEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        args.Handled = true;

        EnsureComp<VampireRegenComponent>(uid);

        RemoveStaminaCrit(uid);
        OnActionUsed(uid, component, args);
    }

    private void OnVampireRejuvenate(EntityUid uid, VampireComponent component, VampireRejuvenateEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        RemoveStaminaCrit(uid);
        OnActionUsed(uid, component, args);

        args.Handled = true;
    }

    private void RemoveStaminaCrit(EntityUid uid)
    {
        _statusEffects.TryRemoveStatusEffect(uid, "SlowedDown");
        _statusEffects.TryRemoveStatusEffect(uid, "Stun");
        _statusEffects.TryRemoveStatusEffect(uid, "KnockedDown");
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        var vampireQuery = EntityQueryEnumerator<VampireRegenComponent, DamageableComponent, MobStateComponent>();

        while (vampireQuery.MoveNext(out var uid, out var vampire, out var damage, out var mobState))
        {
            if (vampire.NextTick + RegenPeriod > curTime)
                continue;

            vampire.NextTick = curTime;
            vampire.RegenTimes += 1;

            if (mobState.CurrentState == MobState.Alive)
                _damageable.TryChangeDamage(uid, vampire.HealingDamage, true, false);

            if (mobState.CurrentState == MobState.Dead || vampire.RegenTimes == 20)
            {
                RemComp<VampireRegenComponent>(uid);
            }
        }
    }
}

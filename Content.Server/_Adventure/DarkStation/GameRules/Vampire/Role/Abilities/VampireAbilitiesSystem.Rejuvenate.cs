using Content.Shared._Adventure.Vampire;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusEffect;
using VampireComponent = Content.Shared._Adventure.DarkForces.Vampire.Components.VampireComponent;

namespace Content.Server._Adventure.GameRules.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    private static readonly TimeSpan RegenPeriod = TimeSpan.FromSeconds(2);
    [Dependency] private readonly StaminaSystem _staminaSystem = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;

    private void InitRejuvenate()
    {
        SubscribeLocalEvent<VampireComponent, VampireRejuvenateEvent>(OnVampireRejuvenate);
        SubscribeLocalEvent<VampireComponent, VampireRejuvenatePlusEvent>(OnVampireRejuvenatePlusEvent);
    }

    private void OnVampireRejuvenatePlusEvent(EntityUid uid,
        VampireComponent component,
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
        _staminaSystem.ExitStamCrit(uid);
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
                _damageable.TryChangeDamage(uid, vampire.HealingDamage, true, false, damage);

            if (mobState.CurrentState == MobState.Dead || vampire.RegenTimes == 20)
                RemComp<VampireRegenComponent>(uid);
        }
    }
}

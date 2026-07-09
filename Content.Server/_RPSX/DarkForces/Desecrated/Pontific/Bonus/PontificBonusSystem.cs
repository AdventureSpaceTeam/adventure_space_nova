using System;
using Content.Shared.Movement.Systems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.RPSX.DarkForces.Desecrated.Pontific.Bonus;

public sealed class PontificBonusSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PontificBonusComponent, GetMeleeDamageEvent>(OnGetMeleeDamageEvent);
        SubscribeLocalEvent<PontificBonusComponent, RefreshMovementSpeedModifiersEvent>(OnSpeedRefresh);
    }

    private void OnSpeedRefresh(
        EntityUid uid, PontificBonusComponent component, RefreshMovementSpeedModifiersEvent args
    )
    {
        args.ModifySpeed(component.SpeedMultiplier, component.SpeedMultiplier);
    }

    private void OnGetMeleeDamageEvent(EntityUid uid, PontificBonusComponent component, ref GetMeleeDamageEvent args)
    {
        args.Damage *= component.DamageMultiplier;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var pontificFaith = EntityQueryEnumerator<PontificBonusComponent>();
        while (pontificFaith.MoveNext(out var uid, out var pontificFaithComponent))
        {
            if (pontificFaithComponent.TickToDelete >= _timing.CurTime)
                continue;

            var ev = new PontificBonusEndEvent(pontificFaithComponent.Key);
            RaiseLocalEvent(uid, ev);

            RemComp<PontificBonusComponent>(uid);
            _movementSpeed.RefreshMovementSpeedModifiers(uid);
        }
    }

    public void StartFaith(EntityUid uid, string key, int time, float speedBonus, float damageBonus)
    {
        var pontificFaithComponent = EnsureComp<PontificBonusComponent>(uid);

        pontificFaithComponent.TickToDelete = _timing.CurTime + TimeSpan.FromSeconds(time);
        pontificFaithComponent.DamageMultiplier = damageBonus;
        pontificFaithComponent.SpeedMultiplier = speedBonus;
        pontificFaithComponent.Key = key;

        _movementSpeed.RefreshMovementSpeedModifiers(uid);
    }
}

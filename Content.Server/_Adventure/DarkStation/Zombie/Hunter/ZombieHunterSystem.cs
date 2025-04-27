using System.Numerics;
using Content.Server.Interaction;
using Content.Shared.ActionBlocker;
using Content.Shared._Adventure.Zombie.Hunter;
using Content.Shared.Damage;
using Content.Shared.Humanoid;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Standing;
using Content.Shared.Zombies;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server._Adventure.Zombie.Hunter;

public sealed class ZombieHunterSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly ActionBlockerSystem _blockerSystem = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly InteractionSystem _interaction = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly PhysicsSystem _physicsSystem = default!;
    [Dependency] private readonly StandingStateSystem _standing = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    private readonly TimeSpan AttackTime = TimeSpan.FromSeconds(1);
    private readonly TimeSpan FallbackPeriod = TimeSpan.FromSeconds(2);

    private readonly TimeSpan PrepareTime = TimeSpan.FromSeconds(1);

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ZombieHunterTargetComponent, ComponentInit>(OnTargetInit);
        SubscribeLocalEvent<ZombieHunterTargetComponent, UpdateCanMoveEvent>(OnTargetCanMoveEvent);
        SubscribeLocalEvent<ZombieHunterTargetComponent, MobStateChangedEvent>(OnTargetStateChangedEvent);
        SubscribeLocalEvent<ZombieHunterTargetComponent, PullAttemptEvent>(OnHunterTargetPullAttempt);
        SubscribeLocalEvent<ZombieHunterTargetComponent, EntityZombifiedEvent>(OnTargetZombified);

        SubscribeLocalEvent<ZombieHunterComponent, AltUseInWorldEvent>(OnInteractNoHandEvent);
        SubscribeLocalEvent<ZombieHunterComponent, StartCollideEvent>(OnStartCollide);
        SubscribeLocalEvent<ZombieHunterComponent, UpdateCanMoveEvent>(OnCanMoveEvent);
        SubscribeLocalEvent<ZombieHunterComponent, MobStateChangedEvent>(OnMobStateChangedEvent);
        SubscribeLocalEvent<ZombieHunterComponent, AttackAttemptEvent>(OnAttackAttemptEvent);
        SubscribeLocalEvent<ZombieHunterComponent, PullAttemptEvent>(OnHunterPullAttempt);
    }

    private void OnTargetZombified(EntityUid uid, ZombieHunterTargetComponent component, EntityZombifiedEvent args)
    {
        DropHunterTarget((uid, component));

        if (component.Hunter == EntityUid.Invalid || !TryComp<ZombieHunterComponent>(component.Hunter, out var hunter))
            return;

        DropHunterToIdle((component.Hunter, hunter));
    }

    private void OnHunterTargetPullAttempt(EntityUid uid, ZombieHunterTargetComponent component, PullAttemptEvent args)
    {
        args.Cancelled = true;
    }

    private void OnHunterPullAttempt(EntityUid uid, ZombieHunterComponent component, PullAttemptEvent args)
    {
        args.Cancelled = component.AttackState != HunterAttackState.Idle;
    }

    private void OnAttackAttemptEvent(EntityUid uid, ZombieHunterComponent component, AttackAttemptEvent args)
    {
        if (component.AttackState == HunterAttackState.Attack)
            args.Cancel();
    }

    private void OnMobStateChangedEvent(EntityUid uid, ZombieHunterComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead || component.CurrentTarget == EntityUid.Invalid)
            return;

        if (component.CurrentTarget == EntityUid.Invalid ||
            !TryComp<ZombieHunterTargetComponent>(component.CurrentTarget, out var hunterTarget))
            return;

        DropHunterTarget((component.CurrentTarget, hunterTarget));
        DropHunterToIdle((uid, component));

        UpdateAppearance((uid, component));
    }

    private void OnTargetStateChangedEvent(EntityUid uid,
        ZombieHunterTargetComponent component,
        MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead || component.Hunter == EntityUid.Invalid)
            return;

        if (!TryComp<ZombieHunterComponent>(component.Hunter, out var hunterComponent))
            return;

        DropHunterToIdle((component.Hunter, hunterComponent));
        DropHunterTarget((uid, component));
    }

    private void DropHunterTarget(Entity<ZombieHunterTargetComponent> target)
    {
        RemComp<ZombieHunterTargetComponent>(target);
        _blockerSystem.UpdateCanMove(target);
        _standing.Stand(target);
    }

    private void OnTargetInit(EntityUid uid, ZombieHunterTargetComponent component, ComponentInit args)
    {
        _blockerSystem.UpdateCanMove(uid);
    }

    private void OnTargetCanMoveEvent(EntityUid uid, ZombieHunterTargetComponent component, UpdateCanMoveEvent args)
    {
        args.Cancel();
    }

    private void OnCanMoveEvent(EntityUid uid, ZombieHunterComponent component, UpdateCanMoveEvent args)
    {
        if (component.AttackState == HunterAttackState.Idle)
            return;

        args.Cancel();
    }

    private void OnStartCollide(EntityUid uid, ZombieHunterComponent component, StartCollideEvent args)
    {
        if (!IsTargetValid(args.OtherEntity) || component.AttackState != HunterAttackState.Fly ||
            component.CurrentTarget != args.OtherEntity)
            return;

        _standing.Down(args.OtherEntity, false);

        _physicsSystem.SetBodyStatus(args.OurBody, BodyStatus.OnGround);
        _physicsSystem.SetLinearVelocity(uid, Vector2.Zero);
        _physicsSystem.SetLocalCenter(uid, args.OurBody, args.OtherBody.LocalCenter);


        var targetTransform = Transform(args.OtherEntity);

        _transformSystem.SetCoordinates(uid, targetTransform.Coordinates);
        _transformSystem.SetWorldPositionRotation(uid,
            targetTransform.WorldPosition,
            targetTransform.WorldRotation - 90f);

        var targetComp = EnsureComp<ZombieHunterTargetComponent>(args.OtherEntity);
        targetComp.Hunter = uid;

        component.HunterAttackPeriod = _timing.CurTime;
        component.AttackState = HunterAttackState.Attack;

        UpdateAppearance((uid, component));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<ZombieHunterComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.AttackState == HunterAttackState.Fly)
            {
                if (component.FallBackPeriod > curTime)
                    continue;

                DropHunterToIdle((uid, component));
            }

            if (component.CurrentTarget == EntityUid.Invalid)
                continue;

            switch (component.AttackState)
            {
                case HunterAttackState.Prepare:
                    OnPrepareState(uid, component, curTime);
                    continue;
                case HunterAttackState.Attack:
                    OnAttackState(uid, component, curTime);
                    break;
            }
        }
    }

    private void DropHunterToIdle(Entity<ZombieHunterComponent> hunter)
    {
        if (TryComp<PhysicsComponent>(hunter, out var physics))
            _physicsSystem.SetBodyStatus(physics, BodyStatus.OnGround);

        hunter.Comp.AttackState = HunterAttackState.Idle;
        hunter.Comp.CurrentTarget = EntityUid.Invalid;

        UpdateAppearance(hunter);
        _blockerSystem.UpdateCanMove(hunter);
    }

    private void OnAttackState(EntityUid uid, ZombieHunterComponent component, TimeSpan curTime)
    {
        if (component.HunterAttackPeriod > curTime)
            return;

        _damageable.TryChangeDamage(component.CurrentTarget, component.HunterDamage);
        component.HunterAttackPeriod = curTime + AttackTime;
    }

    private void OnPrepareState(EntityUid uid, ZombieHunterComponent component, TimeSpan curTime)
    {
        if (component.HunterPreparePeriod > curTime)
            return;

        if (!_interaction.InRangeUnobstructed(uid, component.CurrentTarget, 5f, popup: true))
        {
            DropHunterToIdle((uid, component));
            return;
        }

        component.AttackState = HunterAttackState.Fly;
        UpdateAppearance((uid, component));
        HunterFly(uid, component);
    }

    private void HunterFly(EntityUid uid, ZombieHunterComponent component)
    {
        var target = component.CurrentTarget;
        if (!TryComp<PhysicsComponent>(uid, out var hunterPhysics))
            return;

        var targetTransform = Transform(target);
        var hunterTransform = Transform(uid);

        var strong = 10.0f;
        var direction = targetTransform.WorldPosition - hunterTransform.WorldPosition;
        var impulseVector = direction.Normalized() * strong * hunterPhysics.Mass;

        _physicsSystem.ApplyLinearImpulse(uid, impulseVector);
        _physicsSystem.SetBodyStatus(hunterPhysics, BodyStatus.InAir);

        component.FallBackPeriod = _timing.CurTime + FallbackPeriod;
    }

    private void OnInteractNoHandEvent(EntityUid uid, ZombieHunterComponent component, AltUseInWorldEvent args)
    {
        if (!_mobState.IsAlive(uid))
            return;

        if (component.CurrentTarget != EntityUid.Invalid)
            return;

        if (!IsTargetValid(args.Target))
            return;

        if (!_interaction.InRangeUnobstructed(uid, args.Target, 5f, popup: true))
            return;

        component.AttackState = HunterAttackState.Prepare;
        component.HunterPreparePeriod = _timing.CurTime + PrepareTime;
        component.CurrentTarget = args.Target;

        _blockerSystem.UpdateCanMove(uid);
        UpdateAppearance((uid, component));
    }

    private void UpdateAppearance(Entity<ZombieHunterComponent> hunter)
    {
        _appearanceSystem.SetData(hunter, HunterVisuals.State, hunter.Comp.AttackState);

        if (!HasComp<AudioComponent>(hunter.Comp.ActiveSoundEnt))
            _audioSystem.Stop(hunter.Comp.ActiveSoundEnt);

        var sound = hunter.Comp.AttackState switch
        {
            HunterAttackState.Fly => hunter.Comp.FlySound,
            HunterAttackState.Attack => hunter.Comp.AttackSound,
            HunterAttackState.Prepare => hunter.Comp.PrepareSound,
            _ => null,
        };

        if (sound == null)
        {
            hunter.Comp.ActiveSoundEnt = null;
            return;
        }

        hunter.Comp.ActiveSoundEnt = _audioSystem
            .PlayEntity(sound, Filter.Pvs(hunter, entityManager: EntityManager), hunter, true)
            .Value
            .Entity;
    }

    private bool IsTargetValid(EntityUid target)
    {
        return !_mobState.IsDead(target) && HasComp<HumanoidAppearanceComponent>(target) &&
               !HasComp<ZombieComponent>(target);
    }
}

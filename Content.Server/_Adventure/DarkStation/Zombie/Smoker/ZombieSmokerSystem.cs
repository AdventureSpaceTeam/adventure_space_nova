using System.Numerics;
using Content.Server.Interaction;
using Content.Shared.ActionBlocker;
using Content.Shared.AdventureSpace.Zombie.Smoker;
using Content.Shared.AdventureSpace.Zombie.Smoker.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Standing;
using Robust.Server.Audio;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Zombie.Smoker;

public sealed partial class ZombieSmokerSystem : SharedZombieSmokerSystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string SmokeEffect = "ZombieSmokerSmokeEffect";

    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly ActionBlockerSystem _blockerSystem = default!;
    [Dependency] private readonly InteractionSystem _interaction = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly StandingStateSystem _standing = default!;

    private readonly TimeSpan PrepareTime = TimeSpan.FromSeconds(1);

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ZombieSmokerComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<ZombieSmokerComponent, AltUseInWorldEvent>(OnAltUsing);
        SubscribeLocalEvent<ZombieSmokerComponent, CtrlUseInWorldEvent>(OnCtrlUsing);
        SubscribeLocalEvent<ZombieSmokerComponent, CannotRichMessageAttemptEvent>(OnCannotRichMessageAttempt);
        SubscribeLocalEvent<ZombieSmokerComponent, MobStateChangedEvent>(OnSmokerStateChanged);

        InitializeTarget();
    }

    private void OnComponentShutdown(EntityUid uid, ZombieSmokerComponent component, ComponentShutdown args)
    {
        DropSmoker((uid, component));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = Timing.CurTime;
        var query = EntityQueryEnumerator<ZombieSmokerComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.SmokerState != ZombieSmokerState.Prepare || component.SmokerPrepareOffset > curTime)
                continue;

            if (component.CurrentTarget == EntityUid.Invalid)
            {
                DropSmokerToIdle((uid, component));
                continue;
            }

            AttackTarget((uid, component));
        }
    }

    private void DropSmokerToIdle(Entity<ZombieSmokerComponent> smoker)
    {
        smoker.Comp.CurrentTarget = EntityUid.Invalid;
        smoker.Comp.SmokerState = ZombieSmokerState.Idle;
        _blockerSystem.UpdateCanMove(smoker);
    }

    private void OnCannotRichMessageAttempt(EntityUid uid,
        ZombieSmokerComponent component,
        CannotRichMessageAttemptEvent args)
    {
        args.Cancel();
    }

    private void OnSmokerStateChanged(EntityUid uid, ZombieSmokerComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead)
            return;

        if (component.CurrentTarget != EntityUid.Invalid)
            DropSmoker((uid, component));

        Spawn(SmokeEffect, Transform(uid).Coordinates);
    }

    private void OnCtrlUsing(EntityUid uid, ZombieSmokerComponent component, CtrlUseInWorldEvent args)
    {
        if (component.CurrentTarget == EntityUid.Invalid)
            return;

        DropSmoker((uid, component));
    }

    private void OnAltUsing(EntityUid uid, ZombieSmokerComponent component, AltUseInWorldEvent args)
    {
        if (!_mobStateSystem.IsAlive(uid))
            return;

        var target = args.Target;
        if (!IsTargetValid(target) || component.SmokerState != ZombieSmokerState.Idle)
            return;

        if (!_interaction.InRangeUnobstructed(uid, target, 8f, popup: true))
            return;

        if (!TryComp<PhysicsComponent>(target, out _))
            return;

        component.SmokerState = ZombieSmokerState.Prepare;
        component.CurrentTarget = target;
        component.SmokerPrepareOffset = Timing.CurTime + PrepareTime;

        Dirty(uid, component);

        _audioSystem.PlayEntity(component.AttackSound, Filter.Pvs(uid, entityManager: EntityManager), uid, true);
        _blockerSystem.UpdateCanMove(uid);
    }

    private void AttackTarget(Entity<ZombieSmokerComponent> smoker)
    {
        var component = smoker.Comp;
        var target = component.CurrentTarget;

        var smokerTransform = Transform(smoker);
        var targetTransform = Transform(target);
        if (!TryComp<PhysicsComponent>(target, out var targetPhysics) ||
            !_interaction.InRangeUnobstructed((smoker, smokerTransform), (target, targetTransform), 8f, popup: false))
        {
            DropSmokerToIdle(smoker);
            return;
        }

        component.SmokerState = ZombieSmokerState.Attack;

        var smokerTarget = EnsureComp<ZombieSmokerTargetComponent>(target);
        smokerTarget.Smoker = smoker;

        Dirty(smoker, component);
        Dirty(target, smokerTarget);

        _blockerSystem.UpdateCanMove(smoker);
        _standing.Down(target, false);

        Physics.SetBodyStatus(targetPhysics, BodyStatus.OnGround);
        Physics.SetLinearVelocity(target, Vector2.Zero);

        CreateDistanceJoint(smoker, (target, smokerTarget));
    }

    private void DropSmoker(Entity<ZombieSmokerComponent> smoker)
    {
        var target = smoker.Comp.CurrentTarget;
        if (target == EntityUid.Invalid)
        {
            DropSmokerToIdle(smoker);
            return;
        }

        RemComp<ZombieSmokerTargetComponent>(target);

        _standing.Stand(target);
        _blockerSystem.UpdateCanMove(target);

        ClearJoint(smoker, target);
        DropSmokerToIdle(smoker);
        Dirty(smoker);
    }
}

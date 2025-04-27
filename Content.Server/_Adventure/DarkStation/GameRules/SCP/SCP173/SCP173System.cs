using Content.Server.Actions;
using Content.Server.Fluids.EntitySystems;
using Content.Server.GameTicking;
using Content.Server.Ghost;
using Content.Shared._Adventure.SCP._173;
using Content.Shared.Chemistry.Components;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Ghost;
using Content.Shared.Humanoid;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Physics;
using Content.Shared.StatusEffect;
using Content.Shared.Traits.Assorted;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server._Adventure.GameRules.SCP.SCP173;

public sealed class SCP173System : SharedSCP173System
{
    [Dependency] private readonly ActionsSystem _actionSys = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly PuddleSystem _puddleSystem = default!;

    private readonly TimeSpan _spookCooldown = TimeSpan.FromSeconds(10);
    private readonly Dictionary<EntityUid, TimeSpan> _spooksCD = new();
    [Dependency] private readonly StatusEffectsSystem _statusEffectsSystem = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCP173Component, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SCP173Component, ComponentRemove>(OnComponentRemove);

        SubscribeLocalEvent<SCP173Component, AttackAttemptEvent>(OnTryAttack);
        SubscribeLocalEvent<SCP173Component, SCP173ShartEvent>(OnShartAction);
        SubscribeLocalEvent<SCP173Component, SCP173BlindEvent>(OnBlindAction);
    }

    private void OnComponentInit(EntityUid uid, SCP173Component component, ComponentInit args)
    {
        _actionSys.AddAction(uid, ref component.ShartActionUid, component.ShartAction);
        _actionSys.AddAction(uid, ref component.BlindActionUid, component.BlindAction);

        _actionSys.SetEnabled(component.BlindActionUid, false);
    }

    private void OnComponentRemove(EntityUid uid, SCP173Component component, ComponentRemove args)
    {
        _actionSys.RemoveAction(uid, component.BlindActionUid);
        _actionSys.RemoveAction(uid, component.ShartActionUid);

        OnStopLookedAt(uid, component);
    }

    private void OnTryAttack(EntityUid uid, SCP173Component component, AttackAttemptEvent args)
    {
        var target = args.Target.GetValueOrDefault();
        if (!TryComp<SCP173FreezeComponent>(uid, out var scp173FreezeComponent) ||
            !CanAttack(uid, target, scp173FreezeComponent))
        {
            args.Cancel();
            return;
        }

        _audio.PlayPvs(component.KillSoundCollection, target);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<SCP173Component, SCP173FreezeComponent>();
        while (query.MoveNext(out var scp173, out var scp173Component, out var scp173FreezeComponent))
        {
            var wasLookedAt = scp173FreezeComponent.LookedAt;
            var isLookedAt = IsLookedAt(scp173,
                scp173Component.EyeSightRange,
                ref scp173Component.Lookers,
                out var newLookers);

            if (newLookers.Count > 0)
            {
                var validSpooks = new List<EntityUid>();
                var curTime = _gameTiming.CurTime;
                foreach (var ply in newLookers)
                {
                    if (_spooksCD.TryGetValue(ply, out var time) && time > curTime)
                        continue;
                    validSpooks.Add(ply);
                    _spooksCD[ply] = curTime + _spookCooldown;
                }

                if (validSpooks.Count > 0)
                    _audio.PlayEntity(scp173Component.SpooksSoundCollection,
                        Filter.Entities(validSpooks.ToArray()),
                        scp173,
                        false);
            }

            if (isLookedAt == wasLookedAt)
                continue;

            scp173FreezeComponent.LookedAt = isLookedAt;
            Dirty(scp173FreezeComponent);

            if (isLookedAt)
                OnStartLookedAt(scp173, scp173Component);
            else
                OnStopLookedAt(scp173, scp173Component);
        }
    }

    private void OnStartLookedAt(EntityUid uid, SCP173Component component)
    {
        if (TryComp<InputMoverComponent>(uid, out var input))
            input.CanMove = false;

        _physics.SetFixedRotation(uid, true);

        if (!_actionSys.TryGetActionData(component.BlindActionUid, out var blindAction))
            return;

        var curTime = _gameTiming.CurTime;

        _actionSys.SetEnabled(component.BlindActionUid, true);
        _actionSys.SetCooldown(component.BlindActionUid, curTime, curTime + blindAction.UseDelay.GetValueOrDefault());
    }

    private void OnStopLookedAt(EntityUid uid, SCP173Component component)
    {
        _actionSys.SetEnabled(component.BlindActionUid, false);

        if (TryComp<InputMoverComponent>(uid, out var input))
            input.CanMove = true;

        _physics.SetFixedRotation(uid, false);
    }

    private bool IsLookedAt(EntityUid uid, float range, ref List<EntityUid> lookers, out List<EntityUid> newLooks)
    {
        var oldLooks = new List<EntityUid>(lookers);
        lookers.Clear();
        var xform = Comp<TransformComponent>(uid);
        var isLookedAt = false;
        newLooks = new List<EntityUid>();
        foreach (var entity in _lookup.GetEntitiesInRange(xform.MapPosition, range))
        {
            if (!IsValidObserver(entity) || HasComp<TemporaryBlindnessComponent>(entity) || !IsVisible(uid, entity))
                continue;

            lookers.Add(entity);
            isLookedAt = true;
            if (!oldLooks.Contains(entity))
                newLooks.Add(entity);
        }

        return isLookedAt;
    }

    //Не призраки и гуманоиды и не ослепленные
    private bool IsValidObserver(EntityUid ent)
    {
        if (HasComp<GhostComponent>(ent) || !_mobState.IsAlive(ent) || HasComp<PermanentBlindnessComponent>(ent))
            return false;

        if (_statusEffectsSystem.HasStatusEffect(ent, "TemporaryBlindness"))
            return false;

        return HasComp<HumanoidAppearanceComponent>(ent) && HasComp<EyeComponent>(ent);
    }

    private bool IsVisible(EntityUid self, EntityUid target)
    {
        var slfXForm = Transform(self);
        var slfPos = _transform.GetWorldPosition(slfXForm);
        var trgPos = _transform.GetWorldPosition(Transform(target));
        var locPos = slfPos - trgPos;
        var ray = new CollisionRay(trgPos, locPos.Normalized(), (int)CollisionGroup.Opaque);
        var results = _physics.IntersectRay(slfXForm.MapID, ray, locPos.Length(), target, false);
        foreach (var result in results)
        {
            var ent = result.HitEntity;
            if (ent == self)
                return true;
            if (HasComp<OccluderComponent>(ent))
                return false;
        }

        return true;
    }

    private void OnShartAction(EntityUid uid, SCP173Component component, SCP173ShartEvent args)
    {
        var solution = new Solution();
        solution.AddReagent("Nutriment", FixedPoint2.New(8));
        solution.AddReagent("SCP173Blood", FixedPoint2.New(100));

        _puddleSystem.TrySplashSpillAt(uid, Transform(uid).Coordinates, solution, out _);

        args.Handled = true;
    }

    private void OnBlindAction(EntityUid uid, SCP173Component component, SCP173BlindEvent args)
    {
        foreach (var ent in _lookup.GetEntitiesInRange(uid, 6))
        {
            var ghostBoo = new GhostBooEvent();
            if (HasComp<PointLightComponent>(ent))
                RaiseLocalEvent(ent, ghostBoo, true);
        }

        Timer.Spawn(3000,
            () =>
            {
                if (_gameTicker.RunLevel != GameRunLevel.InRound || !uid.IsValid())
                    return;
                var lookerList = component.Lookers;
                var coords = Comp<TransformComponent>(uid).Coordinates;
                foreach (var ply in _lookup.GetEntitiesInRange(uid, 10))
                {
                    if (!IsValidObserver(ply))
                        continue;

                    var plyCords = Comp<TransformComponent>(ply).Coordinates;
                    if (coords.TryDistance(_entMan, plyCords, out var dist))
                    {
                        _statusEffectsSystem.TryAddStatusEffect(ply,
                            TemporaryBlindnessSystem.BlindingStatusEffect,
                            TimeSpan.FromSeconds(lookerList.Contains(ply) ? 8 : 12 - dist),
                            true,
                            "TemporaryBlindness");
                    }

                    _audio.PlayStatic(component.ScaresSoundCollection, ply, coords);
                }

                _audio.PlayStatic(component.ScaresSoundCollection, uid, coords);
            });
        args.Handled = true;
    }
}

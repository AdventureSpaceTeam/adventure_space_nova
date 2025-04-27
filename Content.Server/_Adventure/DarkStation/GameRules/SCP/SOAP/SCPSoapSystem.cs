using System.Numerics;
using Content.Server.Actions;
using Content.Shared.AdventureSpace.SCP.Soap;
using Content.Shared.Slippery;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Server.AdventureSpace.GameRules.SCP.SOAP;

public sealed class SCPSoapSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _actionSys = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly StatusEffectsSystem _statSys = default!;
    [Dependency] private readonly SharedStunSystem _stunSys = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SCPSoapComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SCPSoapComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<SCPSoapComponent, SCPSoapSlipActionEvent>(OnSlipAction);
    }

    private void OnComponentInit(EntityUid uid, SCPSoapComponent component, ComponentInit args)
    {
        _actionSys.AddAction(uid, ref component.SlipActionUid, component.SlipAction);
    }

    private void OnComponentRemove(EntityUid uid, SCPSoapComponent component, ComponentRemove args)
    {
        _actionSys.RemoveAction(uid, component.SlipActionUid);
    }

    private void OnSlipAction(EntityUid uid, SCPSoapComponent component, SCPSoapSlipActionEvent args)
    {
        if (_container.IsEntityInContainer(uid))
            return;

        var isSlipped = TrySlip(args.Target, component.SlipActionForce, component.SlipActionStun);
        if (isSlipped)
            _audio.PlayPvs(component.SlipActionSound, uid);

        args.Handled = isSlipped;
    }

    private bool TrySlip(EntityUid uid, float force, float stunTime)
    {
        if (HasComp<KnockedDownComponent>(uid) || HasComp<NoSlipComponent>(uid))
            return false;

        if (_statSys.HasStatusEffect(uid, "KnockedDown") || !_statSys.CanApplyEffect(uid, "KnockedDown"))
            return false;

        if (!TryComp(uid, out PhysicsComponent? physics))
            return false;

        var velocity = physics.LinearVelocity;
        if (velocity.Length() < 0.1)
            _physics.SetLinearVelocity(uid, new Vector2(0.2f) * force, body: physics);

        _stunSys.TryParalyze(uid, TimeSpan.FromSeconds(stunTime), true);

        return true;
    }
}

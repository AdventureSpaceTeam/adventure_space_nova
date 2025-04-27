using Content.Shared._Adventure.Vampire;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using VampireComponent = Content.Shared._Adventure.DarkForces.Vampire.Components.VampireComponent;

namespace Content.Server._Adventure.GameRules.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    [Dependency] private readonly SharedPhysicsSystem _physicsSystem = default!;

    private void InitCharge()
    {
        SubscribeLocalEvent<VampireComponent, VampireChargeEvent>(OnChargeEvent);
    }

    private void OnChargeEvent(EntityUid uid, VampireComponent component, VampireChargeEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        if (!TryComp<PhysicsComponent>(uid, out var vampireMass))
            return;

        var transform = Transform(uid);

        var strong = 100.0f;
        var direction = args.Target.Position - transform.WorldPosition;
        var impulseVector = direction.Normalized() * strong * vampireMass.Mass;

        _physicsSystem.ApplyLinearImpulse(uid, impulseVector);

        args.Handled = true;
    }
}

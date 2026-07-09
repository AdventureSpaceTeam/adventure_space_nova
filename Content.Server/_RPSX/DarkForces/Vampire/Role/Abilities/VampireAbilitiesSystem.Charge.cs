using Content.Server.RPSX.DarkForces.Vampire.Role.Components;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.RPSX.Vampire;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;

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

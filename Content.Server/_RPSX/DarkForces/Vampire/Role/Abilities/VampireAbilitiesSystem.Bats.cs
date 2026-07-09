using Content.Server.RPSX.DarkForces.Vampire.Role.Components;
using Content.Server.Polymorph.Systems;
using Content.Shared.Polymorph;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.RPSX.Vampire;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    [Dependency] private readonly PolymorphSystem _polymorphSystem = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private const string VampireBat = "MobVampireBat";

    [ValidatePrototypeId<PolymorphPrototype>]
    private const string VampireBatPolymorph = "VampireBatPolymorph";

    private void InitBats()
    {
        SubscribeLocalEvent<VampireComponent, VampirePolymorphEvent>(OnPolymorphEvent);
        SubscribeLocalEvent<VampireComponent, VampireSummonBatsEvent>(OnVampireSummonBatsEvent);
    }

    private void OnVampireSummonBatsEvent(EntityUid uid, VampireComponent component, VampireSummonBatsEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;


        var coordinates = Transform(uid).Coordinates;
        var counter = 0;

        while (counter < 6)
        {
            counter++;
            Spawn(VampireBat, coordinates);
        }

        OnActionUsed(uid, component, args);
        args.Handled = true;
    }

    private void OnPolymorphEvent(EntityUid uid, VampireComponent component, VampirePolymorphEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        _polymorphSystem.PolymorphEntity(uid, VampireBatPolymorph);
        OnActionUsed(uid, component, args);
        args.Handled = true;
    }
}

using Content.Server.Polymorph.Systems;
using Content.Shared._Adventure.Vampire;
using Content.Shared.Polymorph;
using Robust.Shared.Prototypes;
using VampireComponent = Content.Shared._Adventure.DarkForces.Vampire.Components.VampireComponent;

namespace Content.Server._Adventure.GameRules.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string VampireBat = "MobVampireBat";

    [ValidatePrototypeId<PolymorphPrototype>]
    private const string VampireBatPolymorph = "VampireBatPolymorph";

    [Dependency] private readonly PolymorphSystem _polymorphSystem = default!;

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

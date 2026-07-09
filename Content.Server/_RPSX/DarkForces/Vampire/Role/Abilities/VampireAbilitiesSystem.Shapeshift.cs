using Content.Server.Humanoid;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.RPSX.Vampire;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    [Dependency] private readonly HumanoidAppearanceSystem _humanoid = default!;

    private void InitShapeshift()
    {
        SubscribeLocalEvent<VampireComponent, VampireShapeshiftEvent>(OnVampireShapeshiftEvent);
    }

    private void OnVampireShapeshiftEvent(EntityUid uid, VampireComponent component, VampireShapeshiftEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        if (!TryComp(uid, out HumanoidAppearanceComponent? humanoid) || !string.IsNullOrEmpty(humanoid.Initial))
            return;

        var profile = HumanoidCharacterProfile.RandomWithSpecies(humanoid.Species);
        _humanoid.LoadProfile(uid, profile, humanoid);

        OnActionUsed(uid, component, args);
        args.Handled = true;
    }
}

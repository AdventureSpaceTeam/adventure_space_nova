using Content.Server.Humanoid;
using Content.Shared._Adventure.Vampire;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using VampireComponent = Content.Shared._Adventure.DarkForces.Vampire.Components.VampireComponent;

namespace Content.Server._Adventure.GameRules.Vampire.Role.Abilities;

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

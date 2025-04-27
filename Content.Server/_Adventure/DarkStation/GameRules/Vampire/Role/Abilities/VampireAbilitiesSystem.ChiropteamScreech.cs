using System.Linq;
using Content.Shared.AdventureSpace.Vampire;
using Content.Shared.AdventureSpace.Vampire.Attempt;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Tag;
using Robust.Shared.Map;
using VampireComponent = Content.Shared.AdventureSpace.DarkForces.Vampire.Components.VampireComponent;

namespace Content.Server.AdventureSpace.GameRules.Vampire.Role.Abilities;

public sealed partial class VampireAbilitiesSystem
{
    private const string WindowTag = "Window";

    private static readonly DamageSpecifier WindowsDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            { "Slash", 500 },
        },
    };

    private static readonly TimeSpan DefaultParalyzeTime = TimeSpan.FromSeconds(5);
    [Dependency] private readonly TagSystem _tagSystem = default!;

    private void InitChiropteamScreech()
    {
        SubscribeLocalEvent<VampireComponent, VampireChiropteanScreechEvent>(OnVampireChiropteanScreechEvent);
    }

    private void OnVampireChiropteanScreechEvent(EntityUid uid,
        VampireComponent component,
        VampireChiropteanScreechEvent args)
    {
        if (args.Handled || !CanUseAbility(component, args))
            return;

        args.Handled = true;

        var coordinates = Transform(uid).Coordinates;

        TryParalyzePlayers(uid, component, coordinates);
        TryDestroyWindows(coordinates);

        OnActionUsed(uid, component, args);
    }

    private void TryParalyzePlayers(EntityUid uid, VampireComponent component, EntityCoordinates coordinates)
    {
        var players =
            _entityLookupSystem.GetEntitiesInRange<HumanoidAppearanceComponent>(coordinates,
                SharedChatSystem.VoiceRange);

        foreach (var player in players)
        {
            var targetUid = player.Owner;
            if (targetUid == uid)
                continue;

            var attemptEvent = new VampireChiropteanScreechAttemptEvent(targetUid, uid, component.FullPower);
            RaiseLocalEvent(targetUid, attemptEvent, true);

            if (attemptEvent.Cancelled)
                continue;

            _stunSystem.TryParalyze(targetUid, DefaultParalyzeTime, true);
        }
    }

    private void TryDestroyWindows(EntityCoordinates coordinates)
    {
        var windows = _entityLookupSystem
            .GetEntitiesInRange<TagComponent>(coordinates, 5)
            .Where(ent => _tagSystem.HasTag(ent.Owner, WindowTag));

        foreach (var window in windows)
        {
            _damageable.TryChangeDamage(window.Owner, WindowsDamage, true, false);
        }
    }
}

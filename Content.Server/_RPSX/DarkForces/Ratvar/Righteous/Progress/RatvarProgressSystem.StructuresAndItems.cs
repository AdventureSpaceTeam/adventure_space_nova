using System;
using Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities.Enchantment;
using Content.Server.RPSX.DarkForces.Ratvar.Righteous.Structures;
using Content.Server.Stunnable;
using Content.Shared.Item;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Progress;

public sealed partial class RatvarProgressSystem
{
    [Dependency] private readonly StunSystem _stunSystem = default!;

    private void InitializeStructuresAndItems()
    {
        SubscribeLocalEvent<RatvarItemComponent, GettingPickedUpAttemptEvent>(OnGettingInteractedAttempt);
        SubscribeLocalEvent<RatvarStructureComponent, ActivatableUIOpenAttemptEvent>(OnActivatableUIAttempt);
    }

    private void OnGettingInteractedAttempt(EntityUid uid, RatvarItemComponent component,
        GettingPickedUpAttemptEvent args)
    {
        if (!HasComp<ItemComponent>(uid))
            return;

        if (!CanUseRatvarItems(args.User))
        {
            _stunSystem.TryAddParalyzeDuration(args.User, TimeSpan.FromSeconds(4));
            args.Cancel();
        }
    }

    private void OnActivatableUIAttempt(EntityUid uid, RatvarStructureComponent component,
        ActivatableUIOpenAttemptEvent args)
    {
        if (!CanUseRatvarItems(args.User))
        {
            args.Cancel();
        }
    }
}

using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared._Adventure.Hands;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Hands;

public sealed partial class NoInteractionInHandsSystem : EntitySystem
{
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<NoInteractionInHandsComponent, UseAttemptEvent>(OnUseAttempt);
    }

    private bool Blocked(EntityUid ent)
    {
        var transform = Transform(ent);
        var parent = transform.ParentUid;

        return _hands.IsHolding(parent, ent);
    }

    private void OnUseAttempt(Entity<NoInteractionInHandsComponent> ent, ref UseAttemptEvent args)
    {
        if (!args.Cancelled && Blocked(ent))
            args.Cancel();
    }
}

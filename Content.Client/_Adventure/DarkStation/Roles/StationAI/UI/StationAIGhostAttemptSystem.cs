using Content.Shared.AdventureSpace.Roles.StationAI.Components;
using Content.Shared.Interaction.Events;

namespace Content.Client.AdventureSpace.Roles.StationAI.UI;

public sealed class StationAIGhostAttemptSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationAIGhostComponent, CannotRichMessageAttemptEvent>(OnRichAttemptEvent);
    }

    private void OnRichAttemptEvent(EntityUid uid,
        StationAIGhostComponent component,
        CannotRichMessageAttemptEvent args)
    {
        args.Cancel();
    }
}

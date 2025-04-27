using Content.Shared._Adventure.Roles.StationAI.Components;
using Content.Shared.Interaction.Events;

namespace Content.Client._Adventure.Roles.StationAI.UI;

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

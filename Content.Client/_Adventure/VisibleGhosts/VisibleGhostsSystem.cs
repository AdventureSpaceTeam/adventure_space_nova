using Content.Client.Ghost;
using Content.Shared._Adventure.VisibleGhosts;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client._Adventure.VisibleGhosts;

public sealed partial class VisibleGhostsSystem : EntitySystem
{
    [Dependency] private readonly GhostSystem _ghost = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VisibleGhostsComponent, LocalPlayerAttachedEvent>(OnPlayerAttach);
    }

    private void OnPlayerAttach(EntityUid uid, VisibleGhostsComponent component, LocalPlayerAttachedEvent localPlayerAttachedEvent)
    {
        _ghost.ToggleGhostVisibility(true);
    }
}

using Content.Shared._Adventure.Medical.Surgery;
using Content.Shared._Adventure.Medical.Surgery.Events.Doll;
using Content.Shared.Body.Part;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using SecretCCVars = Content.Shared._Adventure.CCVars.SecretCCVars;

namespace Content.Client._Adventure.Medical.Surgery.UI.Doll.Widgets.Systems;

public sealed class TargetDollSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _configuration = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public event EventHandler<(BodyPartType?, BodyPartSymmetry)>? SyncTargetPart;
    public event EventHandler? Dispose;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TargetDollComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<TargetDollComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
    }

    private void OnPlayerAttached(Entity<TargetDollComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        if (!_configuration.GetCVar(SecretCCVars.IsTargetDollEnabled))
            return;

        SyncTargetPart?.Invoke(this, (ent.Comp.TargetBodyPart, ent.Comp.BodyPartSymmetry));
    }

    private void OnPlayerDetached(Entity<TargetDollComponent> ent, ref LocalPlayerDetachedEvent args)
    {
        Dispose?.Invoke(this, EventArgs.Empty);
    }

    public void OnTagetBodyPartChanged(BodyPartType? type, BodyPartSymmetry symmetry)
    {
        if (!_configuration.GetCVar(SecretCCVars.IsTargetDollEnabled))
            return;

        if (_player.LocalEntity is not { } userEnt)
            return;

        var @event = new TargetDollChangedEvent
        {
            Entity = GetNetEntity(userEnt),
            Type = type,
            Symmetry = symmetry,
        };

        RaiseNetworkEvent(@event);
    }
}

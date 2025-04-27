using Content.Shared.AdventureSpace.Medical.Surgery;
using Content.Shared.AdventureSpace.Medical.Surgery.Events.Doll;

namespace Content.Server.AdventureSpace.Medical.Surgery.Doll;

public sealed class TargetDollSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<TargetDollChangedEvent>(OnTargetDollChanged);
    }

    private void OnTargetDollChanged(TargetDollChangedEvent ev)
    {
        var ent = GetEntity(ev.Entity);
        if (!TryComp<TargetDollComponent>(ent, out var component))
            return;

        component.TargetBodyPart = ev.Type;
        component.BodyPartSymmetry = ev.Symmetry;

        Dirty(ent, component);
    }
}

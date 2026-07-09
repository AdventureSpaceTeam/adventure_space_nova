using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;

namespace Content.Shared.RPSX.DarkForces.Narsi.Buildings;

public abstract class SharedFakeNarsiDoorSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<FakeDoorCheckPlayerEvent>(OnFakeFoorChecked);
        SubscribeLocalEvent<SharedFakeNarsiDoorComponent, ComponentGetState>(OnFakeNarsiDoorGetState);
    }

    private void OnFakeNarsiDoorGetState(EntityUid uid, SharedFakeNarsiDoorComponent component, ref ComponentGetState args)
    {
        args.State = new SharedFakeNarsiDoorComponentState(component.FakeRsiPath, component.RealRsiPath);
    }

    private void OnFakeFoorChecked(FakeDoorCheckPlayerEvent args)
    {
        if (HasComp<NarsiCultistComponent>(GetEntity(args.Entity)))
        {
            args.IsCultist = true;
            return;
        }
        args.IsCultist = false;
    }
}

[Serializable, NetSerializable]
public sealed class SharedFakeNarsiDoorComponentState : ComponentState
{
    public string FakeRsiPath { get; init; }
    public string RealRsiPath { get; init; }
    public SharedFakeNarsiDoorComponentState(string fakeRsiPath, string realRsiPath)
    {
        FakeRsiPath = fakeRsiPath;
        RealRsiPath = realRsiPath;
    }
}

[Serializable, NetSerializable]
public sealed class FakeDoorCheckPlayerEvent : EntityEventArgs
{
    public NetEntity Entity;
    public bool IsCultist;

    public FakeDoorCheckPlayerEvent(NetEntity entity, bool cultist = false)
    {
        Entity = entity;
        IsCultist = cultist;
    }
}

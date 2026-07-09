using Content.Server.RPSX.DarkForces.Saint.Chaplain.Components;
using Content.Server.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.DarkForces.Saint.Chaplain;

public sealed partial class ChaplainSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChaplainComponent, ComponentInit>(OnChaplainInit);
        SubscribeLocalEvent<ChaplainComponent, ComponentShutdown>(OnChaplainShutdown);

        InitializeNarsi();
        InitializeVampire();
        InitializeAbilities();
        InitializeForceWall();
    }

    private void OnChaplainInit(EntityUid uid, ChaplainComponent component, ComponentInit args)
    {
        _actions.AddAction(uid, ref component.GreatPrayerActionEntity, component.GreatPrayerAction, uid);
        _actions.AddAction(uid, ref component.DefenceBarrierActionEntity, component.DefenceBarrierAction, uid);
        _actions.AddAction(uid, ref component.ExorcismActionEntity, component.ExorcismAction, uid);
    }

    private void OnChaplainShutdown(EntityUid uid, ChaplainComponent component, ComponentShutdown args)
    {
        _actions.RemoveAction(uid, component.GreatPrayerActionEntity);
        _actions.RemoveAction(uid, component.DefenceBarrierActionEntity);
        _actions.RemoveAction(uid, component.ExorcismActionEntity);
    }
}

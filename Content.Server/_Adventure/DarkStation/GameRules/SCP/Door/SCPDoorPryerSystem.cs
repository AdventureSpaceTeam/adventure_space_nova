using Content.Shared.Actions;
using Content.Shared._Adventure.SCP;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;

namespace Content.Server._Adventure.GameRules.SCP.Door;

public sealed class SCPDoorPryerSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedDoorSystem _doorSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCPDoorPryerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SCPDoorPryerComponent, ComponentShutdown>(OnComponentShutDown);
        SubscribeLocalEvent<SCPDoorPryerComponent, SCPOpenDoorEvent>(OnOpenDoorEvent);
    }

    private void OnComponentInit(EntityUid uid, SCPDoorPryerComponent component, ComponentInit args)
    {
        _actionsSystem.AddAction(uid, ref component.PryerActionUid, component.PryerAction);
    }

    private void OnComponentShutDown(EntityUid uid, SCPDoorPryerComponent component, ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(uid, component.PryerActionUid);
    }

    private void OnOpenDoorEvent(EntityUid uid, SCPDoorPryerComponent component, SCPOpenDoorEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<DoorComponent>(args.Target, out var doorComponent) || doorComponent is not
                { ClickOpen: true, State: DoorState.Closed })
            return;

        _doorSystem.StartOpening(args.Target, doorComponent, uid);
        args.Handled = true;
    }
}

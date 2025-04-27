using Content.Server.Mind;
using Content.Server.SurveillanceCamera;
using Content.Shared.AdventureSpace.Roles.StationAI.Components;
using Content.Shared.AdventureSpace.Roles.StationAI.UI;

namespace Content.Server.AdventureSpace.Roles.StationAI;

public sealed partial class StationAISystem
{
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly SurveillanceCameraSystem _surveillanceCameraSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private void InitializeCameras()
    {
        Subs.BuiEvents<StationAIGhostComponent>(
            uiKey: StationAICamerasUiKey.Key,
            subscriber: subs =>
            {
                subs.Event<StationAISelectedCamera>(OnStationSelectCamera);
                subs.Event<StationAIRequestCameraList>(OnCamerasRequested);
                subs.Event<StationAIRequestBackToBody>(OnBackToBodyRequest);
            }
        );

        SubscribeLocalEvent<StationAICarrierComponent, SurveillanceCameraDeactivateEvent>(OnCameraDeactivated);
    }

    private void OnCameraDeactivated(Entity<StationAICarrierComponent> ent, ref SurveillanceCameraDeactivateEvent args)
    {
        OnSharedCameraDeactivated(ent);
    }

    private void OnBackToBodyRequest(Entity<StationAIGhostComponent> ent, ref StationAIRequestBackToBody args)
    {
        BackGhostToCore(ent);
    }

    private void OnStationSelectCamera(Entity<StationAIGhostComponent> ent, ref StationAISelectedCamera args)
    {
        var camera = GetEntity(args.Camera);
        if (!camera.IsValid() || !TryComp<SurveillanceCameraComponent>(camera, out var cameraComponent) ||
            !cameraComponent.Active)
            return;

        if (ent.Comp.ActiveCamera != EntityUid.Invalid)
            DropCamera(ent);

        AttachCamera(ent, camera);
    }

    protected override void DropCamera(Entity<StationAIGhostComponent> ent)
    {
        if (ent.Comp.ActiveCamera.IsValid() && HasComp<SurveillanceCameraComponent>(ent.Comp.ActiveCamera))
            _surveillanceCameraSystem.RemoveActiveViewer(ent.Comp.ActiveCamera, ent, ent);

        base.DropCamera(ent);
    }

    private void AttachCamera(Entity<StationAIGhostComponent> ent, EntityUid camera)
    {
        if (HasComp<SurveillanceCameraComponent>(camera))
            _surveillanceCameraSystem.AddActiveViewer(camera, ent, ent);

        AttachAICamera(ent, camera);
    }

    private void OnCamerasRequested(EntityUid uid, StationAIGhostComponent component, StationAIRequestCameraList args)
    {
        if (!_userInterfaceSystem.HasUi(uid, StationAICamerasUiKey.Key))
            return;

        var aiTransform = Transform(uid);
        var cameraList = new List<StationAICameraUIModel>();
        var cameras = EntityManager.EntityQueryEnumerator<SurveillanceCameraComponent, TransformComponent>();
        while (cameras.MoveNext(out var cameraUid, out var cameraComp, out var cameraTransform))
        {
            if (cameraTransform.MapID != aiTransform.MapID)
                continue;

            var model = new StationAICameraUIModel(
                camera: GetNetEntity(cameraUid),
                coordinates: GetNetCoordinates(cameraTransform.Coordinates),
                available: cameraComp.Active
            );
            cameraList.Add(model);
        }

        var state = new StationAICamerasInterfaceState(cameraList);
        _userInterfaceSystem.SetUiState(uid, StationAICamerasUiKey.Key, state);
    }
}

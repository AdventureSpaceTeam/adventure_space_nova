using Content.Shared.AdventureSpace.Roles.StationAI.UI;

namespace Content.Client.AdventureSpace.Roles.StationAI.UI.Cameras;

/// <summary>
///     Initializes a <see cref="StationAICameraList" /> and updates it when new server messages are received.
/// </summary>
public sealed class StationAICamerasInterface : BoundUserInterface
{
    private StationAICameras? _window;

    public StationAICamerasInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        if (!EntMan.TryGetComponent(Owner, out TransformComponent? xForm))
            return;

        _window = new StationAICameras(xForm.GridUid);

        if (State != null)
            UpdateState(State);

        _window.OpenCentered();

        _window.TryUpdateCameraList += () => SendMessage(new StationAIRequestCameraList());
        _window.BackToBodyAction += () => SendMessage(new StationAIRequestBackToBody());
        _window.WarpToCamera += uid => SendMessage(new StationAISelectedCamera(uid));
    }

    /// <summary>
    ///     Update the UI state based on server-sent info
    /// </summary>
    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not StationAICamerasInterfaceState cast)
            return;

        _window.UpdateCameraList(cast.Cameras);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        _window?.Dispose();
    }
}

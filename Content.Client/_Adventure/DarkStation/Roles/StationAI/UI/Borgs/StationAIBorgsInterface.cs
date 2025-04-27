using Content.Shared.AdventureSpace.Roles.StationAI.UI;

namespace Content.Client.AdventureSpace.Roles.StationAI.UI.Borgs;

public sealed class StationAIBorgsInterface : BoundUserInterface
{
    private StationAIBorgsWindow? _window;

    public StationAIBorgsInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        if (_window is not null)
            return;

        _window = new StationAIBorgsWindow();
        _window.OnClose += Close;
        _window.OpenCentered();
        _window.TryUpdateBorgList += () => SendMessage(new StationAIRequestBorgsList());
        _window.BackToBodyEvent += () => SendMessage(new StationAIRequestBackToBody());
        _window.UseCamera += uid => SendMessage(new StationAIBorgCameraRequest(uid));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not StationAIBorgInterfaceState cast)
            return;

        _window?.UpdateBorgs(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        _window?.Close();
        _window?.Dispose();
    }
}

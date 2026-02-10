using Content.Shared._Adventure.PlantAnalyzer;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Adventure.PlantAnalyzer.UI;

[UsedImplicitly]
public sealed class PlantAnalyzerBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private PlantAnalyzerWindow? _window;

    public PlantAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) {}

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<PlantAnalyzerWindow>();
        _window.OnToggled += AdvPressed;
    }

    protected override void ReceiveMessage(BoundUserInterfaceMessage message)
    {
        if (_window == null)
            return;

        if (message is not PlantAnalyzerScannedSeedPlantInformation cast)
            return;
        _window.Populate(cast);
    }

    public void AdvPressed(bool scanMode)
    {
        SendMessage(new PlantAnalyzerSetMode(scanMode));
    }
}

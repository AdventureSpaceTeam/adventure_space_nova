using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Content.Shared.DeviceLinking;
using Content.Shared._Adventure.EnergyCores;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Client._Adventure.EnergyCores;

public sealed class ComputerEnergyCoreControlBoundUserInterface : BoundUserInterface
{
    private ComputerEnergyCoreControlWindow? _window;

    public ComputerEnergyCoreControlBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) {}

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<ComputerEnergyCoreControlWindow>();
        _window.OnPowerToggleButton += value => SendMessage(new EnergyCoreConsoleIsOnMessage(value));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (_window == null || state is not EnergyCoreConsoleUpdateState cast) return;
        _window.SetTimeOfLife(cast.TimeOfLife);
        _window.SetPower(cast.IsOn);
        _window.SetDamage(cast.CurDamage);
    }
}

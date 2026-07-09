using System;
using Content.Shared.RPSX.DarkForces.Narsi.Buildings.Altar.Abilities;
using Robust.Shared.GameObjects;

namespace Content.Client.RPSX.DarkForces.Narsi.Buildings.Altar.Abilities;

public sealed class NarsiAbilitiesBoundInterface : BoundUserInterface
{
    private NarsiAbilitiesWindow? _window;

    public NarsiAbilitiesBoundInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window?.Dispose();
        _window = new NarsiAbilitiesWindow(this);
        _window.OnClose += Close;
        _window.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _window?.Dispose();
        }
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not NarsiAbilitiesState abilitiesState || _window == null)
            return;

        _window.UpdateState(abilitiesState);
    }

    public void OnAbilityOpen(string id)
    {
        SendMessage(new NarsiAbilityOpenMessage(id));
    }

    public void OnAbilityLearn(string id)
    {
        SendMessage(new NarsiAbilityLearnMessage(id));
    }
}

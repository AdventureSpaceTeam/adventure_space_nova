using Content.Client.Eui;
using Content.Shared._Adventure.DarkForces.Vampire;
using Content.Shared.Eui;
using Robust.Shared.Prototypes;

namespace Content.Client._Adventure.DarkForces.Vampire;

public sealed class VampireAbilitiesEUI : BaseEui
{
    private readonly VampireAbilitiesWindow _window;
    private NetEntity _netEntity = NetEntity.Invalid;

    public VampireAbilitiesEUI()
    {
        _window = new VampireAbilitiesWindow();
        _window.OnClose += OnClosed;
        _window.OnLearnButtonPressed += OnAbilitySelected;
    }

    public override void Opened()
    {
        base.Opened();

        _window.OpenCentered();
    }

    public override void Closed()
    {
        base.Closed();
        _window.Close();
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is not VampireAbilitiesState data)
            return;

        _netEntity = data.NetEntity;
        _window.UpdateState(data);
    }

    private void OnClosed()
    {
        SendMessage(new CloseEuiMessage());
    }

    private void OnAbilitySelected(EntProtoId? replaceId, string actionId, int bloodRequired)
    {
        SendMessage(new VampireAbilitySelected(_netEntity, replaceId, actionId, bloodRequired));
    }
}

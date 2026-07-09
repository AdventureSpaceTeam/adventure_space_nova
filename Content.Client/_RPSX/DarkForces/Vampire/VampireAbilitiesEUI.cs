using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.RPSX.DarkForces.Vampire;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client.RPSX.DarkForces.Vampire;

public sealed class VampireAbilitiesEUI : BaseEui
{
    private NetEntity _netEntity = NetEntity.Invalid;
    private readonly VampireAbilitiesWindow _window;

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

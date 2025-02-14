using Content.Client.Eui;
using Content.Shared.Cloning;
using Content.Shared.Eui;
using Content.Shared._Adventure.Administration.SponsorChange;
using Content.Shared._Adventure.Sponsors;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;
using System.Linq;

namespace Content.Client._Adventure.Administration.SponsorChange;

[UsedImplicitly]
public sealed class SponsorChangeEui : BaseEui
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly IClyde _clyde = default!;

    private readonly ISawmill _sawmill;

    private readonly SponsorChangeWindow _window;

    public SponsorChangeEui()
    {
        IoCManager.InjectDependencies(this);
        _sawmill = _log.GetSawmill("sponsor.changes");
        var protos = _proto.EnumeratePrototypes<SponsorTierPrototype>()
            .Select(x => new ProtoId<SponsorTierPrototype>(x.ID)).ToList();
        _window = new SponsorChangeWindow(protos);
        _window.OnUsernameEntered += (username) => SendMessage(
            new SponsorChangeEuiStateMsg.GetPlayerSponsorInfoRequest(username));
        _window.OnTierSelected += (tier) => SendMessage(
            new SponsorChangeEuiStateMsg.SetSponsorTierRequest(_window.GetUsername(), tier));
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void Opened()
    {
        _clyde.RequestWindowAttention();
        _window.OpenCentered();
    }

    public override void Closed()
    {
        _window.Close();
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is not SponsorChangeEuiState s)
            return;

        _window.SetPlayerName(s.Username);
        _window.SetMenuEnabled(s.IsValidUser);
        if (s.IsValidUser)
            _window.SelectSponsorTier(s.Tier);
    }
}

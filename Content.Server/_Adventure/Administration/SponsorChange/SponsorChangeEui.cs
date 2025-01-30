using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Cloning;
using Content.Shared.Eui;
using Content.Shared.Mind;
using Content.Shared._Adventure.Administration.SponsorChange;
using Content.Server._Adventure.Sponsors;
using Content.Shared._Adventure.Sponsors;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Administration.SponsorChange;

public sealed class SponsorChangeEui : BaseEui
{
    [Dependency] private readonly IAdminManager _admins = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly SponsorsManager _sponsors = default!;

    private readonly ISawmill _sawmill;

    private string _username = string.Empty;
    private bool _isValid = false;
    private ProtoId<SponsorTierPrototype>? _tier = null;

    public SponsorChangeEui()
    {
        IoCManager.InjectDependencies(this);
        _sawmill = _log.GetSawmill("sponsor.changes");
    }

    public override EuiStateBase GetNewState()
    {
        return new SponsorChangeEuiState(_username, _isValid, _tier);
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);
        switch (msg)
        {
            case SponsorChangeEuiStateMsg.SetSponsorTierRequest r:
                SetSponsor(r.Username, r.Tier);
                break;
            case SponsorChangeEuiStateMsg.GetPlayerSponsorInfoRequest r:
                GetSponsorInfo(r.Username);
                break;
        }
    }

    public async void SetSponsor(string username, ProtoId<SponsorTierPrototype>? tier)
    {
        _username = username;
        var player = await _db.GetPlayerRecordByUserName(username);
        if (player == null)
        {
            _isValid = false;
            StateDirty();
            return;
        }
        _db.SetPlayerRecordSponsor(player.UserId, tier?.Id);
        if (tier is null)
        {
            _sponsors.Sponsors[player.UserId] = null;
        }
        else
        {
            var tierProto = _proto.Index<SponsorTierPrototype>(tier.Value.Id);
            _sponsors.Sponsors[player.UserId] = tierProto;
        }
        _tier = tier?.Id;
        StateDirty();
    }

    public async void GetSponsorInfo(string username)
    {
        _username = username;
        var player = await _db.GetPlayerRecordByUserName(username);
        if (player == null)
        {
            _isValid = false;
            StateDirty();
            return;
        }
        _isValid = true;
        _tier = player.SponsorTier;
        StateDirty();
    }
}

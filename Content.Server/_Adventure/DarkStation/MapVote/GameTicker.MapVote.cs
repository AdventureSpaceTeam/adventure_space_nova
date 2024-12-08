using Content.Server.Voting.Managers;
using Content.Shared.Voting;
using Content.Shared._Adventure.ACVar;
using Robust.Shared.Configuration;

namespace Content.Server.GameTicking;

public sealed partial class GameTicker
{
    [Dependency] private readonly IVoteManager _vote = default!;

    private async void StartNewMapVote()
    {
        if (_cfg.GetCVar(ACVars.LobbyVote))
        {
            _vote.CreateStandardVote(null, StandardVoteType.Map);
            _vote.CreateStandardVote(null, StandardVoteType.Preset);
        }
    }
}

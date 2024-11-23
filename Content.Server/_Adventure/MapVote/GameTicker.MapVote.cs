using Content.Shared.Voting;
using Content.Server.Voting.Managers;

namespace Content.Server.GameTicking;

public sealed partial class GameTicker
{
    [Dependency] private readonly IVoteManager _vote = default!;

    private async void StartNewMapVote()
    {
        _vote.CreateStandardVote(null, StandardVoteType.Map);
        _vote.CreateStandardVote(null, StandardVoteType.Preset);
    }
}

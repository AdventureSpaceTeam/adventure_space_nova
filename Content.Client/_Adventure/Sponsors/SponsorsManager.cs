using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Content.Shared._Adventure.Sponsors;

namespace Content.Client._Adventure.Sponsors;

public sealed class SponsorsManager : ISponsorsManager
{
    [Dependency] private readonly ISharedPlayerManager _player = default!;

    [ViewVariables(VVAccess.ReadWrite)]
    public SponsorTierPrototype? CurrentSponsor = null;

    public void SetSponsor(SponsorTierPrototype? sponsor)
    {
        CurrentSponsor = sponsor;
    }

    public SponsorTierPrototype? GetSponsor(NetUserId? userId)
    {
        if (userId is null)
            return CurrentSponsor;
        if (userId != _player.LocalUser)
            return null;
        return CurrentSponsor;
    }

    public SponsorTierPrototype? GetMySponsor()
    {
        return CurrentSponsor;
    }
}

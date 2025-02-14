using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._Adventure.Sponsors;

public interface ISponsorsManager
{
    SponsorTierPrototype? GetSponsor(NetUserId? userId);
}

using Content.Shared.Eui;
using Content.Shared._Adventure.Sponsors;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Administration.SponsorChange;

[Serializable, NetSerializable]
public sealed class SponsorChangeEuiState : EuiStateBase
{
    public string Username { get; }
    public bool IsValidUser { get; }
    public ProtoId<SponsorTierPrototype>? Tier { get; }

    public SponsorChangeEuiState(string username, bool isValidUser, ProtoId<SponsorTierPrototype>? tier)
    {
        Username = username;
        IsValidUser = isValidUser;
        Tier = tier;
    }
}

public static class SponsorChangeEuiStateMsg
{
    [Serializable, NetSerializable]
    public sealed class SetSponsorTierRequest : EuiMessageBase
    {
        public string Username { get; }
        public ProtoId<SponsorTierPrototype>? Tier { get; }

        public SetSponsorTierRequest(string username, ProtoId<SponsorTierPrototype>? tier)
        {
            Username = username;
            Tier = tier;
        }
    }

    [Serializable, NetSerializable]
    public sealed class GetPlayerSponsorInfoRequest : EuiMessageBase
    {
        public string Username { get; set; }

        public GetPlayerSponsorInfoRequest(string username)
        {
            Username = username;
        }
    }
}

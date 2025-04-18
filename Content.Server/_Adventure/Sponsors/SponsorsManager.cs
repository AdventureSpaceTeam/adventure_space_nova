using Content.Server.Database;
using Content.Server._Adventure.DiscordAuth;
using Content.Shared.CCVar;
using Content.Shared._Adventure.ACVar;
using Content.Shared._Adventure.Sponsors;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Robust.Shared;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Content.Server._Adventure.Sponsors;

public sealed class SponsorsManager : ISponsorsManager
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IEntityManager _ent = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IServerNetManager _net = default!;

    private ISawmill _sawmill = default!;
    private string _guildId = string.Empty;
    private string _botToken = string.Empty;

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly Dictionary<NetUserId, SponsorTierPrototype?> Sponsors = new();

    public Action<INetChannel, ProtoId<SponsorTierPrototype>>? OnSponsorConnected = null;

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("sponsors");
        _cfg.OnValueChanged(ACVars.DiscordSponsorsGuildId, s => _guildId = s, true);
        _cfg.OnValueChanged(ACVars.DiscordSponsorsBotToken, s => _botToken = s, true);
        _net.Connecting += OnConnecting;
        _net.Connected += OnConnected;
    }

    private async Task OnConnecting(NetConnectingArgs e)
    {
        var userId = e.UserId;
        if (Sponsors.ContainsKey(userId))
            return;
        ProtoId<SponsorTierPrototype>? tier = null;
        tier = await GetSponsorTier(userId);
        if (tier is null)
            return;
        _sawmill.Debug($"Found sponsor {userId}");
        if (!_proto.TryIndex<SponsorTierPrototype>(tier, out var proto))
            return;
        Sponsors[userId] = proto;
    }

    private void OnConnected(object? sender, NetChannelArgs e)
    {
        if (!Sponsors.TryGetValue(e.Channel.UserId, out var sponsor) || sponsor is null)
            return;
        _sawmill.Debug($"Sponsor connected, invoking connection action");
        OnSponsorConnected?.Invoke(e.Channel, sponsor.ID);
    }

    private async Task<ProtoId<SponsorTierPrototype>?> GetSponsorTier(NetUserId userId)
    {
        var player = await _db.GetPlayerRecordByUserId(userId);
        if (player != null && !string.IsNullOrEmpty(player.SponsorTier))
            return player.SponsorTier;
        if (string.IsNullOrEmpty(player?.DiscordId)) // ds auth probably disabled
            return null;
        if (string.IsNullOrEmpty(_guildId) || string.IsNullOrEmpty(_botToken))
            return null;

        using var getMemberMsg = new HttpRequestMessage(HttpMethod.Get, $"guilds/{_guildId}/members/{player.DiscordId}");
        getMemberMsg.Headers.Authorization = new AuthenticationHeaderValue("Bot", _botToken);
        using HttpResponseMessage memberResp = await DiscordAuthBotManager.discordClient.SendAsync(getMemberMsg);
        var memberRespStr = await memberResp.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<MemberResponse>(memberRespStr);
        foreach (var sponsorTier in _proto.EnumeratePrototypes<SponsorTierPrototype>())
        {
            if ((sponsorTier.DiscordRoleId is not null) && (res?.roles?.Contains(sponsorTier.DiscordRoleId) ?? false))
            {
                _sawmill.Info($"Player {userId} got sponsor protoid \"{sponsorTier.ID}\"");
                return sponsorTier.ID;
            }
        }
        return null;
    }

    public int GetAdditionalCharacterSlots(NetUserId userId)
    {
        if (!Sponsors.TryGetValue(userId, out var tier) || tier is null)
            return 0;
        return tier.AdditionalCharacterSlots;
    }

    public SponsorTierPrototype? GetSponsor(NetUserId? userId)
    {
        if (userId is null)
            return null;
        Sponsors.TryGetValue(userId.Value, out var sponsor);
        return sponsor;
    }
}

public record class MemberResponse(List<string>? roles);

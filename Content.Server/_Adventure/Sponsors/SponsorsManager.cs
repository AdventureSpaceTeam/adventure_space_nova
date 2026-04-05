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
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IServerNetManager _net = default!;

    private ISawmill _sawmill = default!;
    private string _guildId = string.Empty;
    private string _botToken = string.Empty;

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly Dictionary<NetUserId, SponsorTierPrototype?> Sponsors = new();

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly Dictionary<NetUserId, List<SubSponsorTierPrototype>> SubSponsors = new();

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
        // I not quiet sure if we shouldn't repopulate player's sponsor everytime he rejoins server.
        if (Sponsors.ContainsKey(userId))
            return;
        await PopulateSponsors(userId); // Awaiting to not cause a race condition with other connecting methods
    }

    private void OnConnected(object? sender, NetChannelArgs e)
    {
        if (!Sponsors.TryGetValue(e.Channel.UserId, out var sponsor) || sponsor is null)
            return;
        _sawmill.Debug($"Sponsor connected, invoking connection action");
        OnSponsorConnected?.Invoke(e.Channel, sponsor.ID);
    }

    public async Task<SponsorTierPrototype?> PopulateSponsors(NetUserId userId)
    {
        var player = await _db.GetPlayerRecordByUserId(userId);
        if (string.IsNullOrEmpty(player?.DiscordId)) // ds auth probably disabled
            return null;
        if (string.IsNullOrEmpty(_guildId) || string.IsNullOrEmpty(_botToken))
            return null;

        using var getMemberMsg = new HttpRequestMessage(HttpMethod.Get, $"guilds/{_guildId}/members/{player.DiscordId}");
        getMemberMsg.Headers.Authorization = new AuthenticationHeaderValue("Bot", _botToken);
        using HttpResponseMessage memberResp = await DiscordAuthBotManager.discordClient.SendAsync(getMemberMsg);
        var memberRespStr = await memberResp.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<MemberResponse>(memberRespStr);
        foreach (var subSponsorTier in _proto.EnumeratePrototypes<SubSponsorTierPrototype>())
        {
            if ((subSponsorTier.DiscordRoleId is not null) &&
                (res?.roles?.Contains(subSponsorTier.DiscordRoleId) ?? false))
            {
                _sawmill.Debug($"Player {userId} got sub sponsor protoid \"{subSponsorTier.ID}\"");
                // I don't like C# not initialized dynamic arrays, but whatever.
                SubSponsors.GetOrNew(userId).Add(subSponsorTier);
            }
        }
        // If you're wondering, why we cache sponsor tier, but not subsponsor tiers, it's because I hate to touch
        // database schema and I would prefer to do it as little as possible. We need a way to override sponsors tiers,
        // but we don't need to override subsponsors usually.
        if (!string.IsNullOrEmpty(player.SponsorTier))
        {
            if (_proto.TryIndex<SponsorTierPrototype>(player.SponsorTier, out var tier))
            {
                Sponsors[userId] = tier;
                return tier;
            }
            _sawmill.Error($"Player {userId} has invalid sponsor tier: {player.SponsorTier}");
        }
        foreach (var sponsorTier in _proto.EnumeratePrototypes<SponsorTierPrototype>())
        {
            if ((sponsorTier.DiscordRoleId is not null) && (res?.roles?.Contains(sponsorTier.DiscordRoleId) ?? false))
            {
                _sawmill.Debug($"Player {userId} got sponsor protoid \"{sponsorTier.ID}\"");
                Sponsors[userId] = sponsorTier;
                return sponsorTier; // Exit on first tier, there's can be only one on a player.
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

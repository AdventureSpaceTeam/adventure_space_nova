using Robust.Shared.Prototypes;
using System.Text.Json.Serialization;
using Robust.Shared.Serialization;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Content.Shared._Adventure.ACVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using Robust.Shared;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using Content.Shared._Adventure.Sponsors;

namespace Content.Server._Adventure.Sponsors;

public sealed class SponsorsManager
{
    [Dependency] private readonly IServerNetManager _netMgr = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    private ISawmill _sawmill = default!;
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };
    private string _apiUrl = string.Empty;

    public readonly Dictionary<NetUserId, SponsorTierPrototype> Sponsors = new();

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("sponsors");
        _cfg.OnValueChanged(ACVars.SponsorApiUrl, s => _apiUrl = s, true);
        _cfg.OnValueChanged(ACVars.SponsorApiToken, v =>
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", v);
        }, true);
        _netMgr.Connecting += OnConnecting;
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
        if (!_proto.TryIndex<SponsorTierPrototype>(tier, out var proto))
            return;
        Sponsors[userId] = proto;
    }

    private async Task<ProtoId<SponsorTierPrototype>?> GetSponsorTier(NetUserId userId)
    {
        if (string.IsNullOrEmpty(_apiUrl))
            return null;

        var url = $"{_apiUrl}/sponsors/?user_id={userId.ToString()}&";
        try {
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _sawmill.Error(
                    "Failed to get player sponsor info from API: [{StatusCode}] {Response}",
                    response.StatusCode,
                    errorText);
                return null;
            }

            var info = await response.Content.ReadFromJsonAsync<SponsorInfo>();
            return info?.Title;
        } catch (TaskCanceledException e) {
            _sawmill.Error(
                $"Can't access sponsor server. Is it down? {e}");
            return null;
        }
    }

    public int GetAdditionalCharacterSlots(NetUserId userId)
    {
        if (!Sponsors.TryGetValue(userId, out var tier))
            return 0;
        return tier.AdditionalCharacterSlots;
    }
}

public sealed class SponsorInfo
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

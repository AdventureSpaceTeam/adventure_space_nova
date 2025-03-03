using Content.Server.Database;
using Content.Shared._Adventure.ACVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Content.Server.Adventure.DiscordAuth;

public sealed class DiscordAuthBotManager
{
    [Dependency] public IConfigurationManager _cfg = default!;
    [Dependency] public IServerDbManager _db = default!;
    [Dependency] public IServerNetManager _net = default!;

    public HttpListener listener = default!;
    public string listeningUrl = string.Empty;
    public string redirectUrl = string.Empty;
    public string clientId = string.Empty;
    public string clientSecret = string.Empty;
    public static HttpClient discordClient = new()
    {
        BaseAddress = new Uri("https://discord.com/api/v10")
    };
    public Dictionary<Guid, NetUserId> stateToUid = new();

    public ISawmill _sawmill = default!;

    public void Initialize()
    {
        _sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("discord_auth");
        _cfg.OnValueChanged(ACVars.DiscordAuthClientId, _ => UpdateAuthHeader(), false);
        _cfg.OnValueChanged(ACVars.DiscordAuthClientSecret, _ => UpdateAuthHeader(), true);
        _cfg.OnValueChanged(ACVars.DiscordAuthListeningUrl, url => listeningUrl = url, true);
        _cfg.OnValueChanged(ACVars.DiscordAuthRedirectUrl, url => redirectUrl = url, true);
        _cfg.OnValueChanged(ACVars.DiscordAuthDebugApiUrl, url => discordClient.BaseAddress = new Uri(url), true);
        listener = new HttpListener();
        listener.Prefixes.Add(listeningUrl);
        listener.Start();
        Task.Run(ListenerThread);
        _net.Connecting += OnConnecting;
    }

    public async Task OnConnecting(NetConnectingArgs e)
    {
        var userId = e.UserId;
        var player = await _db.GetPlayerRecordByUserId(userId);
        if (player is null)
        {
            e.Deny($"User not found.\nПользователь не найден\nuserId: {userId}");
            return;
        }
        if (player.DiscordId is not null) return;
        var link = GenerateInviteLink(userId);
        e.Deny(new NetDenyReason($"Пожалуйста, авторизуйтесь по ссылке", new Dictionary<string, object>
        {
            {"discord_auth_link", link}
        }));
    }

    public void UpdateAuthHeader()
    {
        clientId = _cfg.GetCVar(ACVars.DiscordAuthClientId);
        clientSecret = _cfg.GetCVar(ACVars.DiscordAuthClientSecret);
        discordClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));
    }

    public string GenerateInviteLink(NetUserId uid)
    {
        if (discordClient.BaseAddress is null) return string.Empty;
        var guid = Guid.NewGuid();
        stateToUid[guid] = uid;
        // https://discord.com/oauth2/authorize?client_id=1336163093159481397&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A3963%2F&scope=identify
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("client_id", clientId);
        queryString.Add("response_type", "code");
        queryString.Add("redirect_uri", redirectUrl);
        queryString.Add("scope", "identify");
        queryString.Add("state", guid.ToString());
        var uri = new UriBuilder(new Uri(discordClient.BaseAddress, "oauth2/authorize"));
        uri.Query = queryString.ToString();
        return uri.ToString();
    }

    public void WriteStringStream(HttpListenerResponse resp, string text)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
        resp.ContentLength64 = buffer.Length;
        var output = resp.OutputStream;
        output.Write(buffer,0,buffer.Length);
        output.Close();
    }

    public void errorReturn(HttpListenerResponse resp, string errorString)
    {
        _sawmill.Debug($"Returned {errorString}");
        resp.StatusCode = (int) HttpStatusCode.Unauthorized;
        resp.StatusDescription = "Unauthorized";
        WriteStringStream(resp, errorString);
    }

    public async Task HandleConnection(HttpListenerContext ctx)
    {
        HttpListenerRequest request = ctx.Request;
        using HttpListenerResponse resp = ctx.Response;
        resp.Headers.Set("Content-Type", "text/plain; charset=UTF-8");
        var guidString = request.QueryString.Get("state");
        if (guidString is null)
        {
            errorReturn(resp, "No state found");
            return;
        }
        var guid = new Guid(guidString);
        NetUserId userId = stateToUid[guid];
        stateToUid.Remove(guid); // Don't allow linking multiple accounts to the same uid
        var code = request.QueryString.Get("code");
        if (code is null)
        {
            errorReturn(resp, "No code found");
            return;
        }
        var rqArgs = new Dictionary<string, string>();
        rqArgs["grant_type"] = "authorization_code";
        rqArgs["code"] = code;
        rqArgs["redirect_uri"] = redirectUrl;
        using var getTokenMsg = new HttpRequestMessage(HttpMethod.Post, "oauth2/token")
        {
            Content = new FormUrlEncodedContent(rqArgs),
        };
        using HttpResponseMessage response = await discordClient.SendAsync(getTokenMsg);
        var str = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<TokenResponse>(str);
        if (res is null)
        {
            _sawmill.Error($"Error {str}");
            errorReturn(resp, "Error on connection to discord api");
            return;
        }

        using var getUserMsg = new HttpRequestMessage(HttpMethod.Get, "users/@me");
        getUserMsg.Headers.Authorization = new AuthenticationHeaderValue(res.token_type, res.access_token);
        using HttpResponseMessage userResp = await discordClient.SendAsync(getUserMsg);
        var userRespStr = await userResp.Content.ReadAsStringAsync();
        var userRespRes = JsonSerializer.Deserialize<UserResponse>(userRespStr);
        if (userRespRes is null)
        {
            _sawmill.Error($"Error {userRespStr}");
            errorReturn(resp, "Error on getting user information");
            return;
        }
        var discordId = userRespRes.id;

        if (discordId is null) {
            _sawmill.Error($"Error, can't get discord Id");
            errorReturn(resp, "Error, can't recieve discord id from discord api");
            return;
        }

        if (_db.GetPlayerRecordByDiscordId(discordId) is not null)
        {
            _sawmill.Warning($"Error, {discordId} tried to link account twice");
            errorReturn(resp, "Пользователь уже привязан");
            return;
        }

        if (!(await _db.SetPlayerRecordDiscordId(userId, discordId)))
        {
            _sawmill.Error($"Error, could not found {userId}");
            errorReturn(resp, "Error, non such user");
            return;
        }

        _sawmill.Info($"Player: {userId} linked to discord uid {discordId}");
        resp.StatusCode = (int) HttpStatusCode.OK;
        resp.StatusDescription = "OK";
        WriteStringStream(resp, "Good");
    }

    public async Task ListenerThread()
    {
        while (true)
        {
            try {
                HttpListenerContext ctx = listener.GetContext();
                HandleConnection(ctx);
            } catch (Exception e) {
                _sawmill.Error($"Error handling discord callback:\n{e}");
            }
        }
    }

    // {"token_type": "Bearer", "access_token": "ibnjoi44JCapPRWRDU4EPtE3slJFWC", "expires_in": 604800, "refresh_token": "Ljk03G6mG6du1Lo6yazxrQ6Se7oLY1", "scope": "identify"}
    public record class TokenResponse(
        string token_type = "Bearer",
        string? access_token = null,
        int expires_in = 0,
        string? refresh_token = null,
        string scope = "identify",
        string? state = null);

    // {"id":"642524678136659968","username":"c4llv07e","avatar":"417944fb9465a53484dd8f6b4282c580","discriminator":"0","public_flags":0,"flags":0,"banner":null,"accent_color":null,"global_name":"c4llv07e","avatar_decoration_data":null,"banner_color":null,"clan":null,"primary_guild":null,"mfa_enabled":true,"locale":"en-US","premium_type":0}
    public record class UserResponse(string? id = null);
}

using Content.Shared._Adventure.ACVar;
using Robust.Shared.Configuration;
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
    public ISawmill _sawmill = default!;
    public IConfigurationManager _cfg = default!;
    public HttpListener listener = default!;
    public string listeningUrl = string.Empty;
    public static HttpClient discordClient = new()
    {
        BaseAddress = new Uri("https://discord.com/api/v10")
    };

    public void Initialize()
    {
        _sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("discord_auth");
        _cfg = IoCManager.Resolve<IConfigurationManager>();
        _cfg.OnValueChanged(ACVars.DiscordAuthClientId, _ => UpdateAuthHeader(), false);
        _cfg.OnValueChanged(ACVars.DiscordAuthClientSecret, _ => UpdateAuthHeader(), true);
        _cfg.OnValueChanged(ACVars.DiscordAuthListeningUrl, url => listeningUrl = url, true);
        _cfg.OnValueChanged(ACVars.DiscordAuthDebugApiUrl, url => discordClient.BaseAddress = new Uri(url), true);
        listener = new HttpListener();
        listener.Prefixes.Add(listeningUrl);
        listener.Start();
        Task.Run(ListenerThread);
    }

    public void UpdateAuthHeader()
    {
        var client_id = _cfg.GetCVar(ACVars.DiscordAuthClientId);
        var client_secret = _cfg.GetCVar(ACVars.DiscordAuthClientSecret);
        discordClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{client_id}:{client_secret}")));
    }

    public void WriteStringStream(HttpListenerResponse resp, string text)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
        resp.ContentLength64 = buffer.Length;
        var output = resp.OutputStream;
        output.Write(buffer,0,buffer.Length);
        output.Close();
    }

    public async Task HandleConnection(HttpListenerContext ctx)
    {
        Console.WriteLine("Listening");
            HttpListenerRequest request = ctx.Request;
            using HttpListenerResponse resp = ctx.Response;
            Console.WriteLine("Connected");
            resp.Headers.Set("Content-Type", "text/html");

            var code = request.QueryString.Get("code");
            if (code is null)
            {
                Console.WriteLine("Code not found");
                resp.StatusCode = (int) HttpStatusCode.Unauthorized;
                resp.StatusDescription = "Unauthorized";
                WriteStringStream(resp, "No code found");
                Console.WriteLine("Written");
                return;
            }

            var rqArgs = new Dictionary<string, string>();

            rqArgs["grant_type"] = "authorization_code";
            rqArgs["code"] = code;
            rqArgs["redirect_uri"] = listeningUrl;

            Console.WriteLine("Sending request");
            using var getTokenMsg = new HttpRequestMessage(HttpMethod.Post, "oauth2/token")
            {
                Content = new FormUrlEncodedContent(rqArgs),
            };
            using HttpResponseMessage response = await discordClient.SendAsync(getTokenMsg);
            Console.WriteLine("Request sent");
            var str = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"str: {str}");
            var res = JsonSerializer.Deserialize<TokenResponse>(str);
            if (res is null)
            {
                resp.StatusCode = (int) HttpStatusCode.Forbidden;
                resp.StatusDescription = "Forbidden";
                WriteStringStream(resp, $"Error {str}");
                return;
            }

            Console.WriteLine($"{res.access_token} {res.token_type}");

            using var getUserMsg = new HttpRequestMessage(HttpMethod.Get, "users/@me");
            getUserMsg.Headers.Authorization = new AuthenticationHeaderValue(res.token_type, res.access_token);
            using HttpResponseMessage userResp = await discordClient.SendAsync(getUserMsg);
            var userRespStr = await userResp.Content.ReadAsStringAsync();
            Console.WriteLine($"user resp: {userRespStr}");
            var userRespRes = JsonSerializer.Deserialize<UserResponse>(userRespStr);
            if (userRespRes is null)
            {
                resp.StatusCode = (int) HttpStatusCode.Forbidden;
                resp.StatusDescription = "Forbidden";
                WriteStringStream(resp, $"Error {userRespStr}");
                return;
            }

            Console.WriteLine($"user id: {userRespRes.id}");

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

    public record class OauthTokenRequest(
        string? grant_type = null,
        string? code = null,
        string? redirect_uri = null,
        string? state = null);

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

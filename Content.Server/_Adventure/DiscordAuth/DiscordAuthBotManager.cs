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
            using var msg = new HttpRequestMessage(HttpMethod.Post, "oauth2/token")
            {
                Content = new FormUrlEncodedContent(rqArgs),
            };
            using HttpResponseMessage response = await discordClient.SendAsync(msg);
            Console.WriteLine("Request sent");
            // var str = await response.Content.ReadAsStringAsync();
            // Console.WriteLine($"str: {str}");
            // TODO(c4): ReadFromJson not working, understand why
            var tokenStruct = await response.Content.ReadFromJsonAsync<TokenResponse>();

            Console.WriteLine($"{tokenStruct}, {tokenStruct?.token_type} {tokenStruct?.access_token}");

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
        string grant_type,
        string code,
        string redirect_uri);

    public record class TokenResponse(
        string token_type = "Bearer",
        string? access_token = null,
        int expires_in = 0,
        string? refresh_token = null,
        string scope = "identify");
}

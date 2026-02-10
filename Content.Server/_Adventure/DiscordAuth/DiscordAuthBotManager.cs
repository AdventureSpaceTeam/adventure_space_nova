using Content.Server.Database;
using Content.Shared._Adventure.ACVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Random;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net.WebSockets;
using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System;

namespace Content.Server._Adventure.DiscordAuth;

public sealed class DiscordAuthBotManager
{
    [Dependency] public IConfigurationManager _cfg = default!;
    [Dependency] public IRobustRandom _random = default!;
    [Dependency] public IServerDbManager _db = default!;
    [Dependency] public IServerNetManager _net = default!;

    public HttpListener listener = default!;
    public bool discordAuthEnabled = false;
    public string botToken = string.Empty;
    public string listeningUrl = string.Empty;
    public string redirectUrl = string.Empty;
    public string clientId = string.Empty;
    public string clientSecret = string.Empty;
    public string ManagementRole = string.Empty;
    public string GuildId = string.Empty;
    public string ApplicationId = string.Empty;
    public string ContentFolder = string.Empty;
    public static HttpClient discordClient = new()
    {
        BaseAddress = new Uri("https://discord.com/api/v10")
    };
    public Dictionary<Guid, NetUserId> stateToUid = new();
    public CancellationTokenSource? ListeningCancelation;
    public CancellationTokenSource? CommandListeningCancelation;

    public ISawmill _sawmill = default!;

    public void Initialize()
    {
        _sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("discord_auth");
        _cfg.OnValueChanged(ACVars.DiscordAuthClientId, _ => UpdateAuthHeader(), false);
        _cfg.OnValueChanged(ACVars.DiscordAuthClientSecret, _ => UpdateAuthHeader(), true);
        _cfg.OnValueChanged(ACVars.DiscordAuthManagementRole, managementRole => ManagementRole = managementRole, true);
        _cfg.OnValueChanged(ACVars.DiscordAuthGuildId, guildId => GuildId = guildId, true);
        _cfg.OnValueChanged(ACVars.DiscordAuthApplicationId, applicationId => ApplicationId = applicationId, true);
        _cfg.OnValueChanged(ACVars.DiscordAuthListeningUrl, url => listeningUrl = url, true);
        _cfg.OnValueChanged(ACVars.DiscordBotToken, token => botToken = token, true);
        _cfg.OnValueChanged(ACVars.DiscordAuthRedirectUrl, url => redirectUrl = url, true);
        _cfg.OnValueChanged(ACVars.DiscordAuthDebugApiUrl, url => discordClient.BaseAddress = new Uri(url), true);
        _cfg.OnValueChanged(ACVars.DiscordAuthContentFolder, path => ContentFolder = path, true);

        listener = new HttpListener();
        listener.Prefixes.Add(listeningUrl);
        _cfg.OnValueChanged(ACVars.DiscordAuthEnabled, OnToggledDiscordAuth, true);
    }

    public async Task<T?> TryParse<T>(string value)
    {
        try {
            return JsonSerializer.Deserialize<T>(value);
        } catch (JsonException) {
            var typeName = typeof(T);
            _sawmill.Error($"Error, can't convert {value} to {typeName}");
        }
        return default;
    }

    public async Task<T?> TryParse<T>(HttpContent content)
    {
        var str = await content.ReadAsStringAsync();
        return await TryParse<T>(str);
    }

    public void OnToggledDiscordAuth(bool val)
    {
        discordAuthEnabled = val;
        if (val)
        {
            ListeningCancelation = new CancellationTokenSource();
            CommandListeningCancelation = new CancellationTokenSource();
            listener.Start();
            Task.Run(ListenerThread, ListeningCancelation.Token);
            Task.Run(CommandListenerThread, CommandListeningCancelation.Token);
            _net.Connecting += OnConnecting;
        } else {
            ListeningCancelation?.Cancel();
            listener.Stop();
            _net.Connecting -= OnConnecting;
        }
    }

    public async Task OnConnecting(NetConnectingArgs e)
    {
        if (!discordAuthEnabled) return;
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
        // https://discord.com/oauth2/authorize?client_id=1999999999999999997&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A3963%2F&scope=identify
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
        if ((ListeningCancelation?.Token.IsCancellationRequested) ?? true) return;
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
        var res = await TryParse<TokenResponse>(response.Content);
        if (res is null)
        {
            _sawmill.Error($"Error, can't get token response");
            errorReturn(resp, "Error on connection to discord api");
            return;
        }

        using var getUserMsg = new HttpRequestMessage(HttpMethod.Get, "users/@me");
        getUserMsg.Headers.Authorization = new AuthenticationHeaderValue(res.token_type, res.access_token);
        using HttpResponseMessage userResp = await discordClient.SendAsync(getUserMsg);
        var userRespRes = await TryParse<UserResponse>(userResp.Content);
        if (userRespRes is null)
        {
            _sawmill.Error($"Error, can't get user information");
            errorReturn(resp, "Error on getting user information");
            return;
        }
        var discordId = userRespRes.id;

        if (discordId is null) {
            _sawmill.Error($"Error, can't get discord Id");
            errorReturn(resp, "Error, can't recieve discord id from discord api");
            return;
        }

        var player = await _db.GetPlayerRecordByDiscordId(discordId);

        if (player is not null)
        {
            _sawmill.Warning($"Error, {discordId} ({player.UserId}) tried to link account twice");
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
        while (!(ListeningCancelation?.Token.IsCancellationRequested ?? true))
        {
            try {
                HttpListenerContext ctx = listener.GetContext();
                await HandleConnection(ctx);
            } catch (Exception e) {
                _sawmill.Error($"Error handling discord callback:\n{e}");
                Thread.Sleep((int)20000); // Sleep 20 seconds
            }
        }
    }

    private async Task<string> ReceiveAsyncAll(ClientWebSocket ws, CancellationToken cancel)
    {
        StringBuilder sb = new();
        WebSocketReceiveResult result;
        var buffer = new byte[1024];
        var arrayBuffer = new ArraySegment<byte>(buffer);
        do {
            result = await ws.ReceiveAsync(arrayBuffer, cancel);
            var chunk = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            sb.Append(chunk);
        } while (!result.EndOfMessage);
        return sb.ToString();
    }

    public async Task HeartbeatThread(ClientWebSocket ws, float hb, CancellationToken cancel)
    {
        var rand = _random.NextFloat();
        if (rand < 0.2) rand = 0.2f;
        hb = hb * rand;
        while (!cancel.IsCancellationRequested)
        {
            Thread.Sleep((int)hb);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("{\"op\": 1, \"d\": null}")), WebSocketMessageType.Text, true, cancel);
        }
    }

    public async Task<HttpResponseMessage> SendResponse(string id, string token, string? message = null, bool mention = true, Attachment[]? attachments = null)
    {
        var encMsg = HttpUtility.JavaScriptStringEncode(message);
        var disableMention = mention ? "" : ",\"allowed_mentions\":{\"parse\":[]}";
        var content = message is not null ? $"\"content\":\"{encMsg}\"" : "";
        string attachmentJson = string.Empty;
        int attachmentId = 0;
        if (attachments is not null)
        {
            bool isFirst = true;
            if (!mention || message is not null) attachmentJson += ",";
            attachmentJson += "\"attachments\": [";
            foreach (var attachment in attachments)
            {
                if (!isFirst) attachmentJson += ",";
                attachmentJson += $"{{\"id\": {attachmentId}}}";
                attachmentId += 1;
                isFirst = true;
            }
            attachmentJson += "]";
        }
        var data = $"{{\"type\": 4, \"data\": {{{content}{disableMention}{attachmentJson}}}}}";
        Console.WriteLine($"Sending {data}");
        HttpContent? httpContent = null;
        if (attachments is not null)
        {
            var mp = new MultipartFormDataContent
            {
                {new StringContent(data, Encoding.UTF8, "application/json"), "payload_json"}
            };
            attachmentId = 0;
            foreach (var attachment in attachments)
            {
                mp.Add(new ByteArrayContent(attachment.data), $"files[{attachmentId}]", attachment.filename);
                attachmentId += 1;
            }
            httpContent = mp;
        }
        else
        {
            httpContent = new System.Net.Http.StringContent(data, Encoding.UTF8, "application/json");
        }
        using var request = new HttpRequestMessage(HttpMethod.Post, $"interactions/{id}/{token}/callback")
        {
            Content = httpContent,
        };
        return await discordClient.SendAsync(request);
    }

    public async Task CommandListenerThread()
    {
        if (GuildId == string.Empty)
        {
            _sawmill.Error($"No {ACVars.DiscordAuthGuildId.Name} for command listener");
            return;
        }
        if (ApplicationId == string.Empty)
        {
            _sawmill.Error($"No {ACVars.DiscordAuthApplicationId.Name} for command listener");
            return;
        }
        try {
            while (!(CommandListeningCancelation?.Token.IsCancellationRequested ?? true))
            {
                using (ClientWebSocket ws = new ClientWebSocket())
                {
                    Uri serverUri = new Uri("wss://gateway.discord.gg/?v=10&encoding=json");
                    await ws.ConnectAsync(serverUri, CommandListeningCancelation?.Token ?? default);
                    var result = await ReceiveAsyncAll(ws, CommandListeningCancelation?.Token ?? default);
                    var hello = await TryParse<HelloMessage>(result);
                    if (hello is null || hello.op != 10)
                    {
                        _sawmill.Error($"Error, hello message doesn't contain heartbeat interval");
                        return;
                    }
                    var hb = hello.d?.heartbeat_interval ?? 100000;
                    _sawmill.Info($"Heartbeat interval: {hb}");
                    _ = Task.Run(() => HeartbeatThread(ws, hb, CommandListeningCancelation?.Token ?? default));
                    await ws.SendAsync(
                        new ArraySegment<byte>(Encoding.UTF8.GetBytes($"{{\"op\": 2, \"d\": {{\"token\": \"{botToken}\"," +
                                                                      $"\"intents\": 512, \"properties\": {{\"os\": \"linux\"," +
                                                                      $"\"browser\": \"irc was better\", \"device\": " +
                                                                      $"\"c4lldev\"}}}}}}")),
                        WebSocketMessageType.Text, true,
                        CommandListeningCancelation?.Token ?? default);
                    DiscordCommand[] commands =
                        {
                            new("unlink_discord", 1, "Отвязать аккаунт дискорда от аккаунта игры",
                                new DiscordCommandOption[]{
                                    new("user", "Пользователь, которого надо отвязать", 6, true)
                                }),
                            new("discord_name", 1, "Найти пользователя по нику",
                                new DiscordCommandOption[]{
                                    new("ckey", "Ник пользователя в игре", 3, true)
                                }),
                            new("wyci", 1, "When you code it", Array.Empty<DiscordCommandOption>()),
                            new("wysi", 1, "When you sprite it", Array.Empty<DiscordCommandOption>()),
                            new("wypi", 1, "When you prototype it", Array.Empty<DiscordCommandOption>()),
                        };
                    DiscordCommand[] existingCommands;
                    var uri = $"applications/{ApplicationId}/guilds/{GuildId}/commands";
                    using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                    {
                        request.Headers.Add("Authorization", $"Bot {botToken}");
                        using HttpResponseMessage resp = await discordClient.SendAsync(request);
                        var remoteCommands = await TryParse<DiscordCommand[]>(resp.Content);
                        if (remoteCommands is null)
                        {
                            _sawmill.Error("Can't get remote commands");
                            _sawmill.Debug($"req {uri}");
                            return;
                        }
                        existingCommands = remoteCommands;
                    }
                    foreach (var command in commands)
                    {
                        bool added = false;
                        foreach (var existCommand in existingCommands)
                        {
                            // If you have free life to waste, add options comparision.
                            if (existCommand.name == command.name &&
                                existCommand.type == command.type &&
                                existCommand.description == command.description) added = true;
                        }
                        if (added) continue;
                        _sawmill.Info($"Adding command {command.name}");
                        _sawmill.Debug($"Path: applications/{ApplicationId}/guilds/{GuildId}/commands");
                        string payload = JsonSerializer.Serialize(command);
                        _sawmill.Debug($"Content: {payload}");
                        using var request = new HttpRequestMessage(HttpMethod.Post, $"applications/{ApplicationId}/guilds/{GuildId}/commands")
                        {
                            Content = new System.Net.Http.StringContent(payload, Encoding.UTF8, "application/json"),
                        };
                        request.Headers.Add("Authorization", $"Bot {botToken}");
                        _sawmill.Debug($"Auth: Bot {botToken}");
                        using HttpResponseMessage resp = await discordClient.SendAsync(request);
                        var commandResp = await TryParse<DiscordCommandResponse>(resp.Content);
                        _sawmill.Debug($"Command {command.name}, resp {commandResp}");
                    }
                    while (!(CommandListeningCancelation?.Token.IsCancellationRequested ?? true))
                    {
                        result = await ReceiveAsyncAll(ws, CommandListeningCancelation?.Token ?? default);
                        var msg = await TryParse<GenericMessageType>(result);
                        if (msg is not null && msg.op == 0 && msg.t == "INTERACTION_CREATE")
                        {
                            var interaction = await TryParse<InteractionCreateMessage>(result);
                            if (interaction is not null && interaction.d is not null)
                            {
                                var id = interaction.d.id;
                                var token = interaction.d.token;
                                var options = interaction.d.data?.options;
                                switch (interaction.d.data?.name)
                                {
                                    case "discord_name":
                                        if (options is null || options.Count < 1)
                                        {
                                            await SendResponse(id, token, "Error, no name provided");
                                            break;
                                        }
                                        if (!(interaction.d.member?.roles?.Contains(ManagementRole) ?? false))
                                        {
                                            await SendResponse(id, token, "Error, insufficient rights");
                                            break;
                                        }
                                        var username = options[0].value;
                                        var player = await _db.GetPlayerRecordByUserName(username);
                                        if (player is null)
                                        {
                                            await SendResponse(id, token, "Error, player doesn't exists");
                                            break;
                                        }
                                        var discordId = player.DiscordId;
                                        if (discordId is null)
                                        {
                                            await SendResponse(id, token, "Error, player doesn't have a linked discord account");
                                            break;
                                        }
                                        await SendResponse(id, token, $"<@{discordId}>", mention: false);
                                        break;
                                    case "unlink_discord":
                                        if (options is null || options.Count < 1)
                                        {
                                            await SendResponse(id, token, "Error, no user provided");
                                            break;
                                        }
                                        if (!(interaction.d.member?.roles?.Contains(ManagementRole) ?? false))
                                        {
                                            await SendResponse(id, token, "Error, insufficient rights");
                                            break;
                                        }
                                        discordId = options[0].value;
                                        player = await _db.GetPlayerRecordByDiscordId(discordId);
                                        if (player is null)
                                        {
                                            await SendResponse(id, token, "Error, account doesn't linked to game");
                                            break;
                                        }
                                        if (!await _db.SetPlayerRecordDiscordId(player.UserId, null))
                                        {
                                            await SendResponse(id, token, "Unknown Error");
                                            break;
                                        }
                                        await SendResponse(id, token, "Success!");
                                        break;
                                    case "wyci":
                                    {
                                        string fileName = "wyci.png";
                                        string filePath = Path.Combine(ContentFolder, fileName);
                                        try {
                                            var attachmentData = await File.ReadAllBytesAsync(filePath);
                                            var resp = await SendResponse(id, token, attachments: new Attachment[] {new Attachment(fileName, attachmentData)});
                                        } catch (IOException e) {
                                            _sawmill.Error($"Exception when tried to read {filePath}: {e}");
                                        }
                                    } break;
                                    case "wysi":
                                    {
                                        string fileName = "wysi.png";
                                        string filePath = Path.Combine(ContentFolder, fileName);
                                        try {
                                            var attachmentData = await File.ReadAllBytesAsync(filePath);
                                            var resp = await SendResponse(id, token, attachments: new Attachment[] {new Attachment(fileName, attachmentData)});
                                        } catch (IOException e) {
                                            _sawmill.Error($"Exception when tried to read {filePath}: {e}");
                                        }
                                    } break;
                                    case "wypi":
                                    {
                                        string fileName = "wypi.png";
                                        string filePath = Path.Combine(ContentFolder, fileName);
                                        try {
                                            var attachmentData = await File.ReadAllBytesAsync(filePath);
                                            var resp = await SendResponse(id, token, attachments: new Attachment[] {new Attachment(fileName, attachmentData)});
                                        } catch (IOException e) {
                                            _sawmill.Error($"Exception when tried to read {filePath}: {e}");
                                        }
                                    } break;
                                    default:
                                    {
                                        _sawmill.Error($"Unknown command: {interaction.d.data?.name}");
                                    } break;
                                }
                            }
                        }
                        else if (msg is not null && msg.op == 1)
                        {
                            _sawmill.Debug($"Sending forced heartbeat");
                            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("{\"op\": 1, \"d\": null}")),
                                               WebSocketMessageType.Text, true, CommandListeningCancelation?.Token ?? default);
                        }
                    }
                }
            }
        } catch (Exception e) {
            _sawmill.Error($"Error handling discord gateway:\n{e}");
        }
    }

    // {"token_type": "Bearer", "access_token": "ibnxxxxxxxxxxxxxxxxxxxxxxxxFWC", "expires_in": 604800, "refresh_token": "LjxxxxxxxxxxxxxxxxxxxxxxxxxXY1", "scope": "identify"}
    public record class TokenResponse(
        string token_type = "Bearer",
        string? access_token = null,
        int expires_in = 0,
        string? refresh_token = null,
        string scope = "identify",
        string? state = null);

    // {"id":"642524678136659968","username":"c4llv07e","avatar":"417944fb9465a53484dd8f6b4282c580","discriminator":"0","public_flags":0,"flags":0,"banner":null,"accent_color":null,"global_name":"c4llv07e","avatar_decoration_data":null,"banner_color":null,"clan":null,"primary_guild":null,"mfa_enabled":true,"locale":"en-US","premium_type":0}
    public record class UserResponse(string? id = null);

    public record class HeartbeatReceiveMessage(int heartbeat_interval = 100000);

    // {"t":null,"s":null,"op":10,"d":{"heartbeat_interval":41250,"_trace":["[\"gateway-prd-us-east1-d-7bqh\",{\"micros\":0.0}]"]}
    public record class HelloMessage(int op = 0, HeartbeatReceiveMessage? d = null);
    public record class GenericMessageType(int op = 0, string t = "");

    public record class InteractionCreateMessage(int op = 0, string t = "", InteractionMessageInfo? d = null);
    public record class InteractionMessageInfo(string id = "", string token = "", DiscordMember? member = null, InteractionData? data = null);

    public record class DiscordMember(List<string>? roles = null, string nick = "");
    public record class Attachment(string filename, byte[] data);

    public record class InteractionData(List<InteractionOption>? options = null, string name = "");
    public record class InteractionOption(string value = "");

    public record struct DiscordCommandOption(string name, string description, int type, bool required);
    public record struct DiscordCommand(string name, int type, string description, DiscordCommandOption[] options);

    public record struct DiscordCommandResponse(int id);
}

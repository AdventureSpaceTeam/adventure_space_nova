using Content.Shared._Adventure.DiscordAuth;
using Content.Shared._Adventure.DiscordMember;
using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Shared.Network;
using System.IO;

namespace Content.Client._Adventure.DiscordAuth;

public sealed class DiscordAuthManager
{
    [Dependency] private readonly IClientNetManager _net = default!;
    [Dependency] private readonly IStateManager _state = default!;

    private string _authUrl = string.Empty;
    private Texture? _qrcode;
    private string _discordUrl = string.Empty;
    private Texture? _discordQrcode;
    private string _discordUsername = string.Empty;

    public DiscordAuthManager(string authUrl, string discordUrl, string discordUsername)
    {
        _authUrl = authUrl;
        _discordUrl = discordUrl;
        _discordUsername = discordUsername;
    }

    public string AuthUrl => _authUrl;

    public Texture? Qrcode => _qrcode;

    public string DiscordUrl => _discordUrl;

    public Texture? DiscordQrcode => _discordQrcode;

    public string DiscordUsername => _discordUsername;

    public void Initialize()
    {
        _net.RegisterNetMessage<MsgDiscordAuthCheck>();
        _net.RegisterNetMessage<MsgDiscordAuthRequired>(OnDiscordAuthRequired);
        _net.RegisterNetMessage<MsgDiscordMemberCheck>();
        _net.RegisterNetMessage<MsgDiscordMemberRequired>(OnDiscordMemberRequired);
    }

    private void OnDiscordAuthRequired(MsgDiscordAuthRequired message)
    {
        if (_state.CurrentState is not DiscordAuthState)
        {
            _authUrl = message.AuthUrl;
            if (message.QrCode.Length > 0)
            {
                using var ms = new MemoryStream(message.QrCode);
                _qrcode = Texture.LoadFromPNGStream(ms);
            }

            _state.RequestStateChange<DiscordAuthState>();
        }
    }

    private void OnDiscordMemberRequired(MsgDiscordMemberRequired message)
    {
        if (_state.CurrentState is not DiscordMemberState)
        {
            _discordUrl = message.AuthUrl;
            _discordUsername = message.DiscordUsername;
            if (message.QrCode.Length > 0)
            {
                using var ms = new MemoryStream(message.QrCode);
                _discordQrcode = Texture.LoadFromPNGStream(ms);
            }

            _state.RequestStateChange<DiscordMemberState>();
        }
    }

    public void UpdateQrcodes(Texture? qrcode, Texture? discordQrcode)
    {
        _qrcode = qrcode;
        _discordQrcode = discordQrcode;
    }

    public void SetAuthUrl(string url)
    {
        _authUrl = url;
    }

    public void SetDiscordUsername(string username)
    {
        _discordUsername = username;
    }
}

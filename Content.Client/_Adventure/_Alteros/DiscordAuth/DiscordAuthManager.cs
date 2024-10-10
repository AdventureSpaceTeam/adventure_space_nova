using System.IO;
using Content.Alteros.Interfaces.Client;
using Content.Alteros.Interfaces.Shared;
using Content.Shared.DiscordAuth;
using Content.Shared.DiscordMember;
using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Shared.Network;

namespace Content.Client.DiscordAuth;

public sealed class DiscordAuthManager : SharedDiscordAuthManager
{
    [Dependency] private readonly IClientNetManager _netManager = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;

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
        _netManager.RegisterNetMessage<MsgDiscordAuthCheck>();
        _netManager.RegisterNetMessage<MsgDiscordAuthRequired>(OnDiscordAuthRequired);
        _netManager.RegisterNetMessage<MsgDiscordMemberCheck>();
        _netManager.RegisterNetMessage<MsgDiscordMemberRequired>(OnDiscordMemberRequired);
    }

    private void OnDiscordAuthRequired(MsgDiscordAuthRequired message)
    {
        if (_stateManager.CurrentState is not DiscordAuthState)
        {
            _authUrl = message.AuthUrl;
            if (message.QrCode.Length > 0)
            {
                using var ms = new MemoryStream(message.QrCode);
                _qrcode = Texture.LoadFromPNGStream(ms);
            }

            _stateManager.RequestStateChange<DiscordAuthState>();
        }
    }

    private void OnDiscordMemberRequired(MsgDiscordMemberRequired message)
    {
        if (_stateManager.CurrentState is not DiscordMemberState)
        {
            _discordUrl = message.AuthUrl;
            _discordUsername = message.DiscordUsername;
            if (message.QrCode.Length > 0)
            {
                using var ms = new MemoryStream(message.QrCode);
                _discordQrcode = Texture.LoadFromPNGStream(ms);
            }

            _stateManager.RequestStateChange<DiscordMemberState>();
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

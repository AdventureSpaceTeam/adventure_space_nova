using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Content.Client.Administration.Systems;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Systems.Bwoink;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Mentor;
using Content.Shared.Input;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Input.Binding;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Content.Shared.Players;
using Content.Client.Mind;

namespace Content.Client._RMC14.Mentor;

public sealed class StaffHelpUIController : UIController, IOnSystemChanged<BwoinkSystem>
{
    [Dependency] private readonly AHelpUIController _aHelp = default!;
    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [UISystemDependency] private readonly AudioSystem? _audio = default!;

    public Dictionary<Button, NetUserId> PlayersButtons = new();
    public Dictionary<NetUserId, string> PlayersNames = new();
    private readonly Dictionary<NetUserId, List<MentorMessage>> _messages = new();

    private bool _isMentor;
    private bool _canReMentor;
    private StaffHelpWindow? _staffHelpWindow;
    private MentorHelpWindow? _mentorHelpWindow;
    private MentorWindow? _mentorWindow;
    private string? _mHelpSound;
    private bool _unread;

    public override void Initialize()
    {
        _net.RegisterNetMessage<MentorStatusMsg>(OnMentorStatus);
        _net.RegisterNetMessage<MentorMessagesReceivedMsg>(OnMentorHelpReceived);
        _net.RegisterNetMessage<MentorSendMessageMsg>();
        _net.RegisterNetMessage<MentorHelpMsg>();
        _net.RegisterNetMessage<DeMentorMsg>();
        _net.RegisterNetMessage<ReMentorMsg>();
        _net.RegisterNetMessage<MentorRequestNamesMsg>();
        _net.RegisterNetMessage<MentorGotNamesMsg>(OnGotNames);
        _config.OnValueChanged(RMCCVars.RMCMentorHelpSound, v => _mHelpSound = v, true);
    }

    private void OnGotNames(MentorGotNamesMsg msg)
    {
        PlayersNames = msg.Names;
        UpdateFilters();
    }

    private void OnMentorStatus(MentorStatusMsg msg)
    {
        _isMentor = msg.IsMentor;
        _canReMentor = msg.CanReMentor;

        if (_isMentor)
        {
            _mentorHelpWindow?.Close();
            _net.ClientSendMessage(new MentorRequestNamesMsg());
        }
        else
        {
            _mentorWindow?.Close();
        }
    }

    private void OnMentorHelpReceived(MentorMessagesReceivedMsg msg)
    {
        var other = false;
        foreach (var message in msg.Messages)
        {
            if (message.Author != _player.LocalUser)
                other = true;

            if (_isMentor &&
                _mentorWindow is not { IsOpen: true })
            {
                _unread = true;
                _aHelp.UnreadAHelpReceived();
            }

            _messages.GetOrNew(message.Destination).Add(message);
            if (_mentorWindow is { IsOpen: true })
            {
                MentorAddPlayerButton(message.Destination);

                if (_mentorWindow.SelectedPlayer == message.Destination)
                {
                    _mentorWindow.Messages.AddMessage(CreateMessageLabel(message));
                    _mentorWindow.Messages.ScrollToBottom();
                }

                continue;
            }

            if (_mentorHelpWindow is { IsOpen: true } &&
                _player.LocalUser == message.Destination)
            {
                _mentorHelpWindow.Messages.AddMessage(CreateMessageLabel(message));
                _mentorHelpWindow.Messages.ScrollToBottom();
            }
        }

        if (other)
        {
            _audio?.PlayGlobal(_mHelpSound, Filter.Local(), false);
            _clyde.RequestWindowAttention();

            if (!_isMentor)
            {
                if (OpenWindow(ref _mentorHelpWindow, CreateMentorHelpWindow, () => _mentorHelpWindow = null))
                {
                    _mentorHelpWindow.OpenCentered();
                }
            }
        }
    }

    public void OnSystemLoaded(BwoinkSystem system)
    {
        CommandBinds.Builder
            .BindBefore(ContentKeyFunctions.OpenAHelp,
                InputCmdHandler.FromDelegate(_ => ToggleWindow()),
                typeof(AHelpUIController))
            .Register<StaffHelpUIController>();
    }

    public void OnSystemUnloaded(BwoinkSystem system)
    {
        CommandBinds.Unregister<StaffHelpUIController>();
    }

    public void ToggleWindow()
    {
        if (_staffHelpWindow != null)
        {
            _staffHelpWindow.Close();
            _staffHelpWindow = null;
            SetAHelpButtonPressed(false);
            return;
        }

        SetAHelpButtonPressed(true);
        _staffHelpWindow = new StaffHelpWindow();
        _staffHelpWindow.OnClose += () => _staffHelpWindow = null;
        _staffHelpWindow.OpenCentered();
        UIManager.ClickSound();

        if (_unread)
            _staffHelpWindow.MentorHelpButton.StyleClasses.Add(StyleNano.StyleClassButtonColorRed);

        _staffHelpWindow.AdminHelpButton.OnPressed += _ =>
        {
            _aHelp.Open();
            _staffHelpWindow.Close();
            SetAHelpButtonPressed(false);
        };

        _staffHelpWindow.MentorHelpButton.OnPressed += _ =>
        {
            SetAHelpButtonPressed(false);
            _unread = false;
            if (_isMentor)
            {
                _net.ClientSendMessage(new MentorRequestNamesMsg());
                if (OpenWindow(ref _mentorWindow, CreateMentorWindow, () => _mentorWindow = null))
                {
                    var loginedUsers = _player.SessionsDict.Keys;
                    var usersList = _messages.Keys.Concat(loginedUsers);
                    foreach (var destination in usersList)
                    {
                        MentorAddPlayerButton(destination);
                    }

                    UpdateFilters();

                    _mentorWindow.OpenCentered();
                }
            }
            else
            {
                if (OpenWindow(ref _mentorHelpWindow, CreateMentorHelpWindow, () => _mentorHelpWindow = null))
                {
                    _mentorHelpWindow.OpenCentered();
                }
            }

            _staffHelpWindow.Close();
        };
    }

    private void UpdateFilters()
    {
        if (_mentorWindow is null)
            return;
        foreach (var child in _mentorWindow.Players.Children)
        {
            if (child is Button button)
            {
                if (button.Text?.IndexOf(_mentorWindow.FilterLineEdit.Text, 0, StringComparison.OrdinalIgnoreCase) == -1)
                    button.Visible = false;
                else
                    button.Visible = true;
                if (PlayersButtons.TryGetValue(button, out var player))
                    button.Text = PlayerButtonText(player);
            }
        }
    }

    private MentorHelpWindow CreateMentorHelpWindow()
    {
        var window = new MentorHelpWindow();
        window.ReMentorButton.OnPressed += _ => _net.ClientSendMessage(new ReMentorMsg());
        window.ReMentorButton.Visible = _canReMentor;
        window.Chat.OnTextEntered += args =>
        {
            window.Chat.Clear();
            if (string.IsNullOrWhiteSpace(args.Text))
                return;

            var msg = new MentorHelpMsg() { Message = args.Text };
            _net.ClientSendMessage(msg);
        };

        if (_player.LocalUser is { } local && _messages.TryGetValue(local, out var messages))
        {
            foreach (var message in messages)
            {
                window.Messages.AddMessage(CreateMessageLabel(message));
                window.Messages.ScrollToBottom();
            }
        }

        return window;
    }

    private MentorWindow CreateMentorWindow()
    {
        var window = new MentorWindow();
        window.FilterLineEdit.OnTextEntered += _ => UpdateFilters();
        window.FilterLineEdit.OnTextChanged += _ => UpdateFilters();
        window.DeMentorButton.OnPressed += _ => _net.ClientSendMessage(new DeMentorMsg());
        window.Chat.OnTextEntered += args =>
        {
            var msg = new MentorSendMessageMsg { Message = args.Text, To = window.SelectedPlayer };
            _net.ClientSendMessage(msg);
            window.Chat.Clear();
        };

        return window;
    }

    public string PlayerButtonText(NetUserId player)
    {
        var playerName = player.ToString();
        if (_player.SessionsDict.TryGetValue(player, out var session))
            playerName = session.Name;

        if (PlayersNames.TryGetValue(player, out var charName))
            playerName += $" ({charName})";

        return playerName;
    }

    private void MentorAddPlayerButton(NetUserId player)
    {
        if (_mentorWindow == null)
            return;

        if (_mentorWindow.PlayerDict.TryGetValue(player, out var button))
        {
            button.SetPositionFirst();
            return;
        }

        var playerName = PlayerButtonText(player);

        var playerButton = new Button
        {
            Text = playerName,
            StyleClasses = { "OpenBoth" },
        };
        playerButton.OnPressed += _ =>
        {
            if (_mentorWindow is not { IsOpen: true })
                return;

            _mentorWindow.SelectedPlayer = player;
            _mentorWindow.Messages.Clear();
            _mentorWindow.Chat.Editable = true;
            if (!_messages.TryGetValue(player, out var authorMessages))
                return;

            foreach (var message in authorMessages)
            {
                _mentorWindow.Messages.AddMessage(CreateMessageLabel(message));
                _mentorWindow.Messages.ScrollToBottom();
            }
        };

        _mentorWindow.Players.AddChild(playerButton);
        PlayersButtons[playerButton] = player;
        playerButton.SetPositionFirst();
        _mentorWindow.PlayerDict[player] = playerButton;
    }

    private bool OpenWindow<T>([NotNullWhen(true)] ref T? window, Func<T> create, Action onClose) where T : DefaultWindow
    {
        if (window != null)
            return true;

        window = create();
        window.OnClose += onClose;
        return true;
    }

    private FormattedMessage CreateMessageLabel(MentorMessage message)
    {
        var author = message.AuthorName;
        if (message.IsMentor)
            author = $"[bold][color=red]{author}[/color][/bold]";

        var text = $"{message.Time:HH:mm} {author}: {FormattedMessage.EscapeText(message.Text)}";
        return FormattedMessage.FromMarkupPermissive(text);
    }

    private void SetAHelpButtonPressed(bool pressed)
    {
        if (_aHelp.GameAHelpButton != null)
            _aHelp.GameAHelpButton.Pressed = pressed;

        if (_aHelp.GameAHelpButton != null)
            _aHelp.GameAHelpButton.Pressed = pressed;
    }
}

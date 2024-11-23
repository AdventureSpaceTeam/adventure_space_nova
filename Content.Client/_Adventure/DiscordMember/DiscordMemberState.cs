using Content.Shared._Adventure.DiscordMember;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Network;
using System.Threading;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Client._Adventure.DiscordMember;

public sealed class DiscordMemberState : State
{
    [Dependency] private readonly IUserInterfaceManager _userInterface = default!;
    [Dependency] private readonly IClientNetManager _net = default!;

    private DiscordMemberGui? _gui;
    private readonly CancellationTokenSource _checkTimerCancel = new();

    protected override void Startup()
    {
        _gui = new DiscordMemberGui();
        _userInterface.StateRoot.AddChild(_gui);

        Timer.SpawnRepeating(TimeSpan.FromSeconds(5), () =>
        {
            _net.ClientSendMessage(new MsgDiscordMemberCheck());
        }, _checkTimerCancel.Token);
    }

    protected override void Shutdown()
    {
        _checkTimerCancel.Cancel();
        _gui!.Dispose();
    }
}

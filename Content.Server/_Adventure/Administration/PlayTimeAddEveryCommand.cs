using Content.Server.Administration;
using Content.Server.Administration.Commands;
using Content.Server.Players.PlayTimeTracking;
using Content.Shared.Administration;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Server.Player;
using Robust.Shared.Console;
using System.Linq;

namespace Content.Server._Adventure._Administration;

/// <summary>
/// Adds time to every role
/// </summary>
[AdminCommand(AdminFlags.Moderator)]
public sealed class PlayTimeAddEveryCommand : IConsoleCommand
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly PlayTimeTrackingManager _playTimeTracking = default!;

    public string Command => "playtime_addevery";
    public string Description => Loc.GetString("cmd-playtime_addevery-desc");
    public string Help => Loc.GetString("cmd-playtime_addevery-help", ("command", Command));

    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("cmd-playtime_addevery-error-args"));
            return;
        }

        if (!int.TryParse(args[1], out var minutes))
        {
            shell.WriteError(Loc.GetString("parse-minutes-fail", ("minutes", args[1])));
            return;
        }

        if (!_playerManager.TryGetSessionByUsername(args[0], out var player))
        {
            shell.WriteError(Loc.GetString("parse-session-fail", ("username", args[0])));
            return;
        }

        var allRolesList = CompletionHelper.PrototypeIDs<PlayTimeTrackerPrototype>().ToList();

        /**
		 * guess we really need to instantinate these in case command is changed
		 * cause idk why but dependencies are not resolved so we call it from current shell
		 */
        var overallCommand = new PlayTimeAddOverallCommand();
        var roleCommand = new PlayTimeAddRoleCommand();

        foreach (var item in allRolesList)
        {
            if (item.Value == "Overall")
            {
                shell.ExecuteCommand($"{overallCommand.Command} {player.Name} {minutes}");
            }
            else
            {
                shell.ExecuteCommand($"{roleCommand.Command} {player.Name} {item.Value} {minutes}");
            }
        }

        var overall = _playTimeTracking.GetOverallPlaytime(player);
        var adminPlayer = shell.Player;
        shell.WriteLine(Loc.GetString(
            "cmd-playtime_addevery-succeed",
            ("username", args[0]),
            ("time", overall)));
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(),
                Loc.GetString("cmd-playtime_addevery-arg-user"));

        if (args.Length == 2)
            return CompletionResult.FromHint(Loc.GetString("cmd-playtime_addevery-arg-minutes"));

        return CompletionResult.Empty;
    }
}

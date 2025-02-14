using Content.Server.Administration;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._Adventure.Administration.SponsorChange;

[AdminCommand(AdminFlags.Host)]
public sealed class SponsorChangeCommand : IConsoleCommand
{
    public string Command => "sponsorchange";
    public string Description => "Opens the sponsor override panel.";
    public string Help => "Usage: sponsorchange";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteLine("This does not work from the server console.");
            return;
        }

        var eui = IoCManager.Resolve<EuiManager>();
        var ui = new SponsorChangeEui();
        eui.OpenEui(ui, player);
    }
}

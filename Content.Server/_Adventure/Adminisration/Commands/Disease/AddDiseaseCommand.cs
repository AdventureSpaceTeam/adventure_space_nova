using Content.Server.Administration;
using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.AdventurePrivate._Alteros.Adminisration.Commands.Disease;

[AdminCommand(AdminFlags.Host)]
public sealed class AddDiseaseCommand : LocalizedCommands
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public override string Command => "adddisease";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteLine($"Invalid amount of arguments.{Help}");
            return;
        }

        _entityManager.System<DiseaseSystem>().ForceAddDisease(EntityUid.Parse(args[0]), args[1]);
    }
}

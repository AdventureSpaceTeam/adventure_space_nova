using Content.Server._Adventure.Bank.PDA.Components;
using Content.Server.PDA.Ringer;
using Content.Shared.CartridgeLoader;
using Robust.Shared.Containers;

namespace Content.Server._Adventure.Roles.Salary;

public sealed partial class CrewMemberSalarySystem
{
    private void MakePayDayNotify(EntityUid station)
    {
        var stationTransform = Transform(station);
        var query =
            EntityQueryEnumerator<CartridgeLoaderComponent, RingerComponent, ContainerManagerComponent,
                TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var ringer, out var cont, out var transform))
        {
            if (stationTransform.MapID != transform.MapID)
                continue;

            if (!_cartridgeLoader.TryGetProgram<BankCartridgeComponent>(uid,
                    out _,
                    out _,
                    false,
                    comp,
                    cont))
                continue;

            _ringerSystem.RingerPlayRingtone((uid, ringer));
        }
    }
}

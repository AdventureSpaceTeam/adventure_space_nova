using System.Linq;
using Content.Server.RPSX.DarkForces.Narsi.Progress.Components;
using Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Offering;
using Content.Server.RPSX.DarkForces.Saint.Chaplain.Components;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.DarkForces.Narsi.Runes;

public sealed partial class NarsiRuneSystem
{
    private void ProcessOfferingRune(EntityUid rune)
    {
        var entities = FindHumanoidsNearRune(rune)
            .Where(entity => _mobStateSystem.IsDead(entity) && !HasComp<ChaplainComponent>(entity) && !HasComp<NarsiCultistComponent>(entity))
            .ToList();

        if (!entities.Any())
            return;

        var target = entities.First();
        if (HasComp<NarsiCultOfferingTargetComponent>(target))
        {
            var ev = new NarsiCultOfferingTargetEvent();
            RaiseLocalEvent(target, ref ev);
            Log.Info("Цель на убийство выполнена");
        }

        QueueDel(target);
    }
}

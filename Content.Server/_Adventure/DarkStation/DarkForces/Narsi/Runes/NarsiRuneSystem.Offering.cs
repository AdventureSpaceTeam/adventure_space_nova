using System.Linq;
using Content.Server._Adventure.DarkForces.Narsi.Progress.Components;
using Content.Server._Adventure.DarkForces.Narsi.Progress.Objectives.Offering;
using Content.Server._Adventure.DarkForces.Saint.Chaplain.Components;
using Content.Shared._Adventure.DarkForces.Narsi.Roles;

namespace Content.Server._Adventure.DarkForces.Narsi.Runes;

public sealed partial class NarsiRuneSystem
{
    private void ProcessOfferingRune(EntityUid rune)
    {
        var entities = FindHumanoidsNearRune(rune)
            .Where(entity => _mobStateSystem.IsDead(entity) && !HasComp<ChaplainComponent>(entity) &&
                             !HasComp<NarsiCultistComponent>(entity))
            .ToList();

        if (!entities.Any())
            return;

        var target = entities.First();
        if (HasComp<NarsiCultOfferingTargetComponent>(target))
        {
            var ev = new NarsiCultOfferingTargetEvent();
            RaiseLocalEvent(target, ref ev);
        }

        QueueDel(target);
    }
}

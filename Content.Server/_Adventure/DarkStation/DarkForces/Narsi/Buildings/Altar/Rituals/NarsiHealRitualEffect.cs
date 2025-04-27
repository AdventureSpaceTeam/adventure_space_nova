using Content.Server._Adventure.DarkForces.Narsi.Buildings.Altar.Rituals.Base;
using Content.Shared._Adventure.DarkForces.Narsi.Roles;
using Content.Shared.Rejuvenate;

namespace Content.Server._Adventure.DarkForces.Narsi.Buildings.Altar.Rituals;

[DataDefinition]
public sealed partial class NarsiHealRitualEffect : NarsiRitualEffect
{
    public override void MakeRitualEffect(EntityUid altar,
        EntityUid perfomer,
        NarsiAltarComponent component,
        IEntityManager entityManager)
    {
        var cultists = entityManager.EntityQueryEnumerator<NarsiCultistComponent>();
        while (cultists.MoveNext(out var cultist, out _))
        {
            entityManager.EventBus.RaiseLocalEvent(cultist, new RejuvenateEvent());
        }

        if (component.BuckledEntity == null)
            return;

        entityManager.QueueDeleteEntity(component.BuckledEntity.Value);
        component.BuckledEntity = null;
    }
}

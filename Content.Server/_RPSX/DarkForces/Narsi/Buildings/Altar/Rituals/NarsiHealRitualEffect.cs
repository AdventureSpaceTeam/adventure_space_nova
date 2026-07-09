using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Base;
using Content.Shared.Rejuvenate;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals;

[DataDefinition]
public sealed partial class NarsiHealRitualEffect : NarsiRitualEffect
{
    public override void MakeRitualEffect(EntityUid altar, EntityUid perfomer, NarsiAltarComponent component,
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

using Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals.Base;
using Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;
using Content.Shared.Polymorph;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals;

[DataDefinition]
public sealed partial class NarsiPolymorphRitualEffect : NarsiRitualEffect
{
    [DataField(required: true)]
    public PolymorphConfiguration Configuration = default!;

    [DataField]
    public bool DeleteBuckled;

    [DataField]
    public bool ReturnToAltar;

    public override void MakeRitualEffect(EntityUid altar,
        EntityUid perfomer,
        NarsiAltarComponent component,
        IEntityManager entityManager)
    {
        var polymorphPerformerEvent = new NarsiRequestPolymorphEvent(
            perfomer,
            Configuration,
            ReturnToAltar
        );
        entityManager.EventBus.RaiseLocalEvent(altar, polymorphPerformerEvent);

        if (!DeleteBuckled)
            return;

        entityManager.QueueDeleteEntity(component.BuckledEntity);
        component.BuckledEntity = null;
    }
}

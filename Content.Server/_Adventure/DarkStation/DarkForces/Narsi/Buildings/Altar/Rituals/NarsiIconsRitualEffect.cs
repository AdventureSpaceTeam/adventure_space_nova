using Content.Server._Adventure.DarkForces.Narsi.Buildings.Altar.Rituals.Base;
using Content.Shared._Adventure.DarkForces.Narsi.Buildings.Altar;

namespace Content.Server._Adventure.DarkForces.Narsi.Buildings.Altar.Rituals;

public sealed partial class NarsiIconsRitualEffect : NarsiRitualEffect
{
    public override void MakeRitualEffect(EntityUid altar,
        EntityUid perfomer,
        NarsiAltarComponent component,
        IEntityManager entityManager)
    {
        var netEvent = new NarsiIconsRitualFinishedEvent();
        entityManager.EntityNetManager.SendSystemNetworkMessage(netEvent);
    }
}

using Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals.Base;
using Content.Shared.AdventureSpace.DarkForces.Narsi.Buildings.Altar;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals;

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

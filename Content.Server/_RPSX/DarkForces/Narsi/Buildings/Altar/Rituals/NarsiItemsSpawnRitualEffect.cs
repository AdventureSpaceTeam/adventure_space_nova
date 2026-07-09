using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Base;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals;

[DataDefinition]
public sealed partial class NarsiItemsSpawnRitualEffect : NarsiRitualEffect
{
    [DataField]
    public EntProtoId ItemToSpawn;

    public override void MakeRitualEffect(EntityUid altar, EntityUid perfomer, NarsiAltarComponent component, IEntityManager entityManager)
    {
        if (!entityManager.TryGetComponent<TransformComponent>(altar, out var transform))
            return;

        entityManager.SpawnEntity(ItemToSpawn, transform.Coordinates);
    }
}

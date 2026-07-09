using System.Linq;
using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Base;
using Content.Server.Mind;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Shared.GameObjects;
using Content.Shared.NPC.Systems;
using Content.Server.Actions;
using Content.Shared.Actions;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Content.Shared.NPC.Components;
using System.Collections.Generic;
using Robust.Shared.Log;
using System;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals;
public sealed partial class NarsiChangeMindRitualEffect : NarsiRitualEffect
{
    public override void MakeRitualEffect(EntityUid altar, EntityUid performer, NarsiAltarComponent component, IEntityManager entManager)
    {
        // 1. Проверяем, кто к алтарю приторочен
        if (!component.BuckledEntity.HasValue)
        {
            return;
        }

        var buckled = component.BuckledEntity.Value;
        var mindSys = entManager.System<MindSystem>();
        var factionSys = entManager.System<NpcFactionSystem>();
        var actionsSys = entManager.System<SharedActionsSystem>();
        var tagSys = entManager.System<TagSystem>();


        // 2. Mind check
        if (!mindSys.TryGetMind(performer, out var perfMind, out _) ||
            !mindSys.TryGetMind(buckled, out var targMind, out _))
        {
            return;
        }

        // 3. Factions
        if (!entManager.TryGetComponent<NpcFactionMemberComponent>(performer, out var perfFaction) ||
            !entManager.TryGetComponent<NpcFactionMemberComponent>(buckled, out var buckledFaction))
        {
            return;
        }

        if (perfFaction.Factions == null || buckledFaction.Factions == null)
        {
            return;
        }

        // 4. Swap factions
        var perfFactions = perfFaction.Factions.ToHashSet();
        var buckledFactions = buckledFaction.Factions.ToHashSet();

        factionSys.ClearFactions(performer);
        factionSys.ClearFactions(buckled);
        factionSys.AddFactions(performer, buckledFactions);
        factionSys.AddFactions(buckled, perfFactions);

        // 5. Способности (лидерские и прочие)
        // Здесь вызываем метод сравнения/синхронизации
        TransferActions(performer, buckled, entManager, actionsSys, tagSys);

        // 6. Меняем mind
        mindSys.TransferTo(perfMind, buckled);
        mindSys.TransferTo(targMind, performer);

        // 7. Проверяем, не является ли buckled культистом
        if (!entManager.HasComponent<NarsiCultistComponent>(buckled))
        {
            entManager.RemoveComponent<NarsiCultistComponent>(performer);
            entManager.AddComponent<NarsiCultistComponent>(buckled);
        }

        // 8. Аналогично, если performer был лидером — переносим Leader
        if (entManager.HasComponent<NarsiCultistLeaderComponent>(performer))
        {
            entManager.RemoveComponent<NarsiCultistLeaderComponent>(performer);
            entManager.AddComponent<NarsiCultistLeaderComponent>(buckled);
        }
        else if (entManager.HasComponent<NarsiCultistLeaderComponent>(buckled))
        {
            entManager.RemoveComponent<NarsiCultistLeaderComponent>(buckled);
            entManager.AddComponent<NarsiCultistLeaderComponent>(performer);
        }
    }

    private void TransferActions(EntityUid first, EntityUid second, IEntityManager ent, SharedActionsSystem actionsSys, TagSystem tagSys)
    {
        var firstActions = actionsSys.GetActions(first).ToList().Where(c => tagSys.HasTag(c.Owner, "NarsiAction"));
        var secondActions = actionsSys.GetActions(second).ToList().Where(c => tagSys.HasTag(c.Owner, "NarsiAction"));

        foreach (var fa in firstActions)
        {
            actionsSys.RemoveAction(fa.Owner);
            if (ent.TryGetComponent<MetaDataComponent>(fa.Owner, out var metaData) && metaData.EntityPrototype != null)
                actionsSys.AddAction(second, metaData.EntityPrototype.ID);
        }

        foreach (var sa in secondActions)
        {
            actionsSys.RemoveAction(sa.Owner);
            if (ent.TryGetComponent<MetaDataComponent>(sa.Owner, out var metaData) && metaData.EntityPrototype != null)
                actionsSys.AddAction(first, metaData.EntityPrototype.ID);
        }
    }
}

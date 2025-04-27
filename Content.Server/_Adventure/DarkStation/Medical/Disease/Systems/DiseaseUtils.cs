using Content.Server.AdventureSpace.Medical.Disease.Components;
using Content.Server.AdventureSpace.Medical.Disease.Data;
using Content.Server.AdventureSpace.Medical.Disease.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Medical.Disease.Systems;

public static class DiseaseUtils
{
    public static void SpreadDisease(this Dictionary<DiseaseSpreading, List<ProtoId<DiseasePrototype>>> source,
        Entity<DiseaseTargetComponent> target,
        DiseaseSpreading spreading,
        Action<EntityUid, string> action)
    {
        foreach (var diseaseId in source[spreading])
        {
            if (target.Comp.Diseases.ContainsKey(diseaseId))
                continue;

            action(target, diseaseId);
        }
    }

    public static void SpreadDisease(this Dictionary<DiseaseSpreading, List<ProtoId<DiseasePrototype>>> source,
        HashSet<Entity<DiseaseTargetComponent>> entities,
        DiseaseSpreading spreading,
        Action<EntityUid, string> action)
    {
        foreach (var diseaseId in source[spreading])
        {
            foreach (var entity in entities)
            {
                if (entity.Comp.Diseases.ContainsKey(diseaseId))
                    continue;

                action(entity, diseaseId);
            }
        }
    }

    public static void SaveAddDisease(this Dictionary<DiseaseSpreading, List<ProtoId<DiseasePrototype>>> source,
        DiseaseSpreading spreading,
        string diseaseId)
    {
        if (source.TryGetValue(spreading, out var list))
        {
            list.Add(diseaseId);
            return;
        }

        var newList = new List<ProtoId<DiseasePrototype>>();
        source[spreading] = newList;
    }
}

using Content.Server.AdventureSpace.Medical.Disease.Components;
using Robust.Shared.Random;

namespace Content.Server.AdventureSpace.Medical.Disease.Systems;

public sealed class DiseasesOnSpawnSystem : EntitySystem
{
    [Dependency] private readonly DiseaseSystem _diseaseSystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DiseasesOnSpawnComponent, ComponentInit>(OnInit);
    }

    private void OnInit(Entity<DiseasesOnSpawnComponent> ent, ref ComponentInit args)
    {
        if (ent.Comp.Prob > 0f && !_random.Prob(ent.Comp.Prob))
            return;

        _diseaseSystem.ForceAddDisease(ent, ent.Comp.DiseaseId);
    }
}

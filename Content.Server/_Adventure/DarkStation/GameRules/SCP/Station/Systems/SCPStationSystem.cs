using Content.Server.AdventureSpace.GameRules.SCP.Station.Components;

namespace Content.Server.AdventureSpace.GameRules.SCP.Station.Systems;

public sealed class SCPStationSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCPStationComponent, ComponentInit>(OnSCPStationInit);
    }

    private void OnSCPStationInit(Entity<SCPStationComponent> ent, ref ComponentInit args)
    {
    }
}

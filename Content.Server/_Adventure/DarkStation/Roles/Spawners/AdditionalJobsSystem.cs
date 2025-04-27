using Content.Server.Station.Components;
using Content.Server.Station.Systems;

namespace Content.Server.AdventureSpace.Roles.Spawners;

public sealed class AdditionalJobsSystem : EntitySystem
{
    [Dependency] private readonly StationJobsSystem _stationJobsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationInitializedEvent>(
            OnStationInitialized,
            [typeof(StationSpawningSystem)],
            [typeof(StationJobsSystem)]
        );
    }

    private void OnStationInitialized(StationInitializedEvent msg)
    {
        if (!TryComp<StationJobsComponent>(msg.Station, out var stationJobs))
            return;

        /* Security */
        stationJobs.AddJob("SecurityBorg", 1, 1);
        stationJobs.AddJob("SecurityKnight", 1, 1);
        stationJobs.AddJob("Brigmedic", 1, 1);

        /* Cargo */
        stationJobs.AddJob("CargoLogistician", 1, 1);

        /* Special */
        stationJobs.AddJob("BlueShieldOfficer", 1, 1);

        /* Crimson Lily */
        stationJobs.AddJob("AgentCrimsonLily", 1, 1);
        stationJobs.AddJob("SpIFD", 1, 1);

        /* CentCom */
        stationJobs.AddJob("SpNanoTrasenRepresentative", 1, 1);
        stationJobs.AddJob("SpCCO", 1, 1);
        /*Medical*/
        stationJobs.AddJob("Surger", 2, 2);
        stationJobs.AddJob("Pathologist", 1, 1);

        /* Seniors */
        stationJobs.AddJob("SeniorOfficer", 1, 1);

        _stationJobsSystem.UpdateJobsAvailable();
    }
}

using Content.Server.GameTicking;
using Content.Server.Spawners.Components;

namespace Content.Server.AdventureSpace.Roles.Spawners;

public sealed class AdditionalJobsSpawnSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    private readonly Dictionary<string, string> _rolesSpawner = new();
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayersSpawned);
        SetupSpawners();
    }

    private void SetupSpawners()
    {
        /* Security */
        _rolesSpawner["SecurityKnight"] = "SecurityOfficer";
        _rolesSpawner["SecurityBorg"] = "SecurityOfficer";
        _rolesSpawner["Brigmedic"] = "SecurityOfficer";

        /* Cargo */
        _rolesSpawner["CargoLogistician"] = "CargoTechnician";

        /* Special */
        _rolesSpawner["BlueShieldOfficer"] = "Captain";
        _rolesSpawner["Lawyer"] = "IAA";
        _rolesSpawner["SpCCO"] = "SpCCO";

        /* Medical */
        _rolesSpawner["Surger"] = "MedicalDoctor";
        _rolesSpawner["Pathologist"] = "MedicalDoctor";

        /* Seniors */
        _rolesSpawner["SeniorResearcher"] = "Scientist";
        _rolesSpawner["SeniorPhysician"] = "MedicalDoctor";
        _rolesSpawner["SeniorEngineer"] = "StationEngineer";
        _rolesSpawner["SeniorOfficer"] = "SecurityOfficer";
    }

    private void OnPlayersSpawned(PlayerSpawnCompleteEvent ev)
    {
        if (ev.JobId == null || !_rolesSpawner.ContainsKey(ev.JobId) || ev.LateJoin)
            return;

        var attachedEntity = ev.Player.AttachedEntity;
        if (attachedEntity == null)
            return;

        var targetSpawnPoint = _rolesSpawner[ev.JobId];

        var entityQuery = _entityManager.EntityQueryEnumerator<SpawnPointComponent>();

        var originalSpawner = GetSpawnPoint(entityQuery, ev.JobId);
        if (originalSpawner != null)
            return;

        var alternativeSpawner = GetSpawnPoint(entityQuery, targetSpawnPoint);
        if (alternativeSpawner == null)
            return;

        var spawnPointTransform = Transform(alternativeSpawner.Value);
        var attachedTransform = Transform((EntityUid)attachedEntity);

        _transformSystem.SetCoordinates(attachedEntity.Value, spawnPointTransform.Coordinates);
        _transformSystem.AttachToGridOrMap(attachedEntity.Value, attachedTransform);
    }

    private EntityUid? GetSpawnPoint(EntityQueryEnumerator<SpawnPointComponent> query, string jobId)
    {
        while (query.MoveNext(out var uid, out var point))
        {
            if (point.Job?.Id == jobId)
                return uid;
        }

        return null;
    }
}

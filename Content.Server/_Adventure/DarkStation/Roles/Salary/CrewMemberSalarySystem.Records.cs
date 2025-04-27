using System.Linq;
using Content.Server.StationRecords.Systems;
using Content.Shared.StationRecords;

namespace Content.Server.AdventureSpace.Roles.Salary;

public sealed partial class CrewMemberSalarySystem
{
    private readonly Dictionary<EntityUid, Dictionary<uint, GeneralStationRecord>> _cachedEntries = new();

    private void InitializeRecords()
    {
        SubscribeLocalEvent<AfterGeneralRecordCreatedEvent>(AfterGeneralRecordCreated);
        SubscribeLocalEvent<RecordModifiedEvent>(OnRecordModified);
        SubscribeLocalEvent<RecordRemovedEvent>(OnRecordRemoved);
    }

    private void OnRecordRemoved(RecordRemovedEvent ev)
    {
        UpdateCrewMembersSalariesCache(ev.Key.OriginStation);
    }

    private void OnRecordModified(RecordModifiedEvent ev)
    {
        UpdateCrewMembersSalariesCache(ev.Key.OriginStation);
    }

    private void AfterGeneralRecordCreated(AfterGeneralRecordCreatedEvent ev)
    {
        UpdateCrewMembersSalariesCache(ev.Key.OriginStation);
    }

    private void UpdateCrewMembersSalariesCache(EntityUid station)
    {
        var records = _recordsSystem.GetRecordsOfType<GeneralStationRecord>(station);
        var cachedRecords = new Dictionary<uint, GeneralStationRecord>();

        foreach (var (key, record) in records)
        {
            cachedRecords[key] = record;
        }

        cachedRecords = cachedRecords.OrderBy(e => e.Value.JobTitle).ThenBy(e => e.Value.Name).ToDictionary();
        _cachedEntries[station] = cachedRecords;
    }

    public Dictionary<uint, GeneralStationRecord>? GetStationCrewEntries(EntityUid station)
    {
        return !_cachedEntries.TryGetValue(station, out var entries) ? null : entries;
    }
}

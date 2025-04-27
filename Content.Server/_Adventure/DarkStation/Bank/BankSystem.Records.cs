using System.Diagnostics.CodeAnalysis;
using Content.Shared._Adventure.Bank.Transactions;
using Content.Shared.StationRecords;

namespace Content.Server._Adventure.Bank;

public sealed partial class BankSystem
{
    private bool TryGetRecordByMobUid(EntityUid uid,
        [NotNullWhen(true)] out GeneralStationRecord? record,
        out EntityUid station)
    {
        record = null;
        station = default;

        if (_stationSystem.GetOwningStation(uid) is not { } owningStation)
            return false;

        station = owningStation;

        if (!_idCardSystem.TryFindIdCard(uid, out var idCard))
            return false;

        if (!TryComp<StationRecordKeyStorageComponent>(idCard, out var keyStorage) || keyStorage.Key == null)
            return false;

        if (!_stationRecordsSystem.TryGetRecord(keyStorage.Key.Value, out record) || record.MobEntity == null)
            return false;

        if (GetEntity(record.MobEntity.Value) == uid)
            return true;

        _stationRecordsSystem.ClearRecentlyAccessed(station);
        return false;
    }

    private void UpdateRecordByTransaction(EntityUid uid, BankTransaction transaction)
    {
        if (!TryGetRecordByMobUid(uid, out var record, out var station))
            return;

        record.BankTransactions.Add(transaction);
        _stationRecordsSystem.Synchronize(station);
    }
}

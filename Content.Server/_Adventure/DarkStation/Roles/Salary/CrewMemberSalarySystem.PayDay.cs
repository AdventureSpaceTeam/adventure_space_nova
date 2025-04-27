using System.Linq;
using Content.Server.AdventureSpace.Roles.Salary.Components;
using Content.Shared.AdventureSpace.Bank.Transactions;
using Content.Shared.AdventureSpace.Roles.Salary;
using Content.Shared.Database;
using Content.Shared.StationRecords;

namespace Content.Server.AdventureSpace.Roles.Salary;

public sealed partial class CrewMemberSalarySystem
{
    private void StartStationPrePayDay(Entity<CrewMemberStationSalaryComponent> salaries)
    {
        var station = salaries.Owner;
        var stationRecords = _cachedEntries[station];

        foreach (var record in stationRecords.Values)
        {
            if (record.NetUserId is not { } userId)
                continue;

            if (record.MobEntity is not { } netEntity)
                continue;

            if (record.Salary is not { } salary)
                continue;

            var uid = GetEntity(netEntity);
            if (!CrewMemberHaveSalary(uid) || salary.PrePaidCount >= 2)
                continue;

            var prepaid = (int)Math.Round(salary.Salary * 0.15);
            salary.PrePaid += prepaid;
            salary.PrePaidCount += 1;

            var transaction = _bankSystem.CreateSalaryTransaction(prepaid, BankSalarySource.CentralCommand);
            _bankSystem.TryExecuteTransaction(uid, userId, transaction);
            MakePayDayNotify(station);
        }
    }

    private void StartStationPayDay(EntityUid station)
    {
        foreach (var record in _cachedEntries[station].Values)
        {
            if (record.NetUserId is not { } userId || record.MobEntity is not { } netEntity ||
                record.Salary is not { } salary)
                continue;

            var uid = GetEntity(netEntity);
            if (!CrewMemberHaveSalary(uid))
                continue;

            var crewSalary = CalculateCrewMemberSalary(salary);
            if (crewSalary == 0)
                continue;

            if (_mobStateSystem.IsDead(uid))
            {
                var salaryCount = (int)Math.Round(crewSalary * 0.30);
                var partialTransaction =
                    _bankSystem.CreateSalaryTransaction(salaryCount, BankSalarySource.CentralCommand);
                _bankSystem.TryExecuteTransaction(uid, userId, partialTransaction);
                _adminLogger.Add(LogType.Salary,
                    LogImpact.Medium,
                    $"Added salary to user ${record.NetUserId}; Salary - {salaryCount}");

                continue;
            }

            var transaction = _bankSystem.CreateSalaryTransaction(crewSalary, BankSalarySource.CentralCommand);
            _bankSystem.TryExecuteTransaction(uid, userId, transaction);

            _adminLogger.Add(LogType.Salary,
                LogImpact.Medium,
                $"Added salary to user ${record.NetUserId}; Salary - {crewSalary}");
            MakePayDayNotify(station);
        }
    }

    private int CalculateCrewMemberSalary(CrewSalaryEntry entry)
    {
        var bonuses = entry.SalaryBonuses.Sum(bonus => bonus.Bonus);
        var penalties = entry.SalaryPenalties.Sum(penalty => penalty.Penalty);

        return entry.Salary - entry.PrePaid + bonuses - penalties;
    }

    private bool CrewMemberHaveSalary(EntityUid uid)
    {
        return _mindSystem.TryGetMind(uid, out var mindId, out _) && !_roleSystem.MindIsAntagonist(mindId);
    }


    public void UpdateCrewMemberBonus(EntityUid station, EntityUid changer, int bonus, uint recordKey)
    {
        var stationRecordKey = _sharedStationRecords.Convert((GetNetEntity(station), recordKey));

        if (!_recordsSystem.TryGetRecord<GeneralStationRecord>(stationRecordKey, out var record))
            return;

        if (record.Salary is not { } salary)
        {
            _recordsSystem.ClearRecentlyAccessed(station);
            return;
        }

        if ((float)bonus / salary.Salary > 0.5f)
        {
            _recordsSystem.ClearRecentlyAccessed(station);
            return;
        }

        salary.SalaryBonuses.Add(new CrewSalaryBonus(bonus, ""));
        _recordsSystem.Synchronize(station);

        _adminLogger.Add(LogType.Salary,
            LogImpact.Medium,
            $"Added salary bonus to user ${record.NetUserId}; bonus - {bonus} by {changer}");
    }

    public void UpdateCrewMemberPenalty(EntityUid station, EntityUid changer, int penalty, uint recordKey)
    {
        var stationRecordKey = _sharedStationRecords.Convert((GetNetEntity(station), recordKey));
        if (!_recordsSystem.TryGetRecord<GeneralStationRecord>(stationRecordKey, out var record))
            return;

        record.Salary?.SalaryPenalties.Add(new CrewSalaryPenalty(penalty, ""));
        _recordsSystem.Synchronize(station);
        _adminLogger.Add(LogType.Salary,
            LogImpact.Medium,
            $"Added penalty to user ${record.NetUserId}; penalty - {penalty} by {changer}");
    }
}

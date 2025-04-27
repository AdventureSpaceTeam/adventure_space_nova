using Content.Server._c4llv07e.Bridges;
using Content.Shared.AdventureSpace.Roles.Salary;
using Content.Shared.StationRecords;

namespace Content.Server.AdventureSpace.Roles.Salary;

public sealed class SalaryBridge : ISalaryBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public CrewSalaryEntry? GetCrewMemberSalary(StationRecordKey key, string jobId)
    {
        return _entityManager.System<CrewMemberSalarySystem>().GetCrewMemberSalary(key, jobId);
    }

    public CrewSalaryEntry? GetCrewMemberSalary(EntityUid station, string jobId)
    {
        return _entityManager.System<CrewMemberSalarySystem>().GetCrewMemberSalary(station, jobId);
    }
}

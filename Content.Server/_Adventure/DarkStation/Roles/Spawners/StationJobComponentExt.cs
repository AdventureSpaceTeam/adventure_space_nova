using Content.Server.Station.Components;

namespace Content.Server.AdventureSpace.Roles.Spawners;

public static class StationJobComponentExt
{
    public static void AddJob(this StationJobsComponent component, string job, int roundStartCount, int lateJoinCount)
    {
        component.JobList[job] = roundStartCount;
        component.JobList[job] = lateJoinCount;

        component.MidRoundTotalJobs += lateJoinCount;
        component.TotalJobs += component.MidRoundTotalJobs;
    }
}

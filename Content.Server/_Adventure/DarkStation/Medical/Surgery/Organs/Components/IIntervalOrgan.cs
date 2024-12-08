namespace Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Organs.Components;

public interface IIntervalOrgan
{
    public TimeSpan NextCheckTime { get; set; }

    public TimeSpan CheckInterval { get; set; }
}

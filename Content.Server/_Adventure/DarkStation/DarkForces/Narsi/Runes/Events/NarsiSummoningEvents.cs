namespace Content.Server._Adventure.DarkForces.Narsi.Runes.Events;

public sealed class NarsiSummoningStartEvent : CancellableEntityEventArgs
{
    public NarsiSummoningStartEvent(EntityUid source)
    {
        Source = source;
    }

    public EntityUid Source { get; }
}

public sealed class NarsiSummoningCanceledEvent : CancellableEntityEventArgs
{
    public NarsiSummoningCanceledEvent(EntityUid source)
    {
        Source = source;
    }

    public EntityUid Source { get; }
}

public sealed class NarsiSummoningEndEvent : CancellableEntityEventArgs
{
    public NarsiSummoningEndEvent(EntityUid source, EntityUid narsiUid)
    {
        Source = source;
        NarsiUid = narsiUid;
    }

    public EntityUid Source { get; }
    public EntityUid NarsiUid { get; }
}

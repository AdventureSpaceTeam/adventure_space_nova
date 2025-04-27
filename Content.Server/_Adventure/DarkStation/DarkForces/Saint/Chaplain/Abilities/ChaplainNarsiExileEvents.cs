namespace Content.Server.AdventureSpace.DarkForces.Saint.Chaplain.Abilities;

public sealed class ChaplainNarsiExileEnableEvent : CancellableEntityEventArgs
{
    public ChaplainNarsiExileEnableEvent(EntityUid chaplain)
    {
        Chaplain = chaplain;
    }

    public EntityUid Chaplain { get; }
}

public sealed class ChaplainNarsiExileStartEvent : CancellableEntityEventArgs
{
    public ChaplainNarsiExileStartEvent(EntityUid chaplain)
    {
        Chaplain = chaplain;
    }

    public EntityUid Chaplain { get; }
}

public sealed class ChaplainNarsiExileCanceledEvent : CancellableEntityEventArgs
{
    public ChaplainNarsiExileCanceledEvent(EntityUid chaplain)
    {
        Chaplain = chaplain;
    }

    public EntityUid Chaplain { get; }
}

public sealed class ChaplainNarsiExileFinishedEvent : CancellableEntityEventArgs
{
    public ChaplainNarsiExileFinishedEvent(EntityUid chaplain)
    {
        Chaplain = chaplain;
    }

    public EntityUid Chaplain { get; }
}

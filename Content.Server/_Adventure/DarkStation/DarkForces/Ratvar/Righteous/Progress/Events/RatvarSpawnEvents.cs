namespace Content.Server._Adventure.DarkForces.Ratvar.Righteous.Progress.Events;

[ByRefEvent]
public record RatvarSpawnStartedEvent(EntityUid Portal);

[ByRefEvent]
public record RatvarSpawnedEvent(EntityUid Ratvar);

[ByRefEvent]
public record RatvarSpawnCanceledEvent;

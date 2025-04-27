namespace Content.Server._Adventure.Medical.Surgery.Events;

[ByRefEvent]
public record struct SurgeryBodyUpdated(EntityUid Body);

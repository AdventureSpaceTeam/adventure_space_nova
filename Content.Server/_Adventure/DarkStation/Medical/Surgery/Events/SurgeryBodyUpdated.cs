namespace Content.Server.AdventureSpace.Medical.Surgery.Events;

[ByRefEvent]
public record struct SurgeryBodyUpdated(EntityUid Body);

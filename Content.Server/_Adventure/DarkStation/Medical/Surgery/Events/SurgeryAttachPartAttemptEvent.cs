namespace Content.Server.AdventureSpace.Medical.Surgery.Events;

[ByRefEvent]
public record struct SurgeryAttachPartAttemptEvent(bool Canceled, string Reason);

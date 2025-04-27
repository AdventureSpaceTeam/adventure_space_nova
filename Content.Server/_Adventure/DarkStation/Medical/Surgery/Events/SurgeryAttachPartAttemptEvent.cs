namespace Content.Server._Adventure.Medical.Surgery.Events;

[ByRefEvent]
public record struct SurgeryAttachPartAttemptEvent(bool Canceled, string Reason);

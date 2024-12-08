namespace Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Events;

[ByRefEvent]
public record struct SurgeryAttachPartAttemptEvent(bool Canceled, string Reason);

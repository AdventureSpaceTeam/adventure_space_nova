using Content.Shared.Body.Part;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Events;

[ByRefEvent]
public readonly record struct SurgeryBodyPartAddedEvent(string Slot, BodyPartComponent Part);

[ByRefEvent]
public readonly record struct SurgeryBodyPartRemovedEvent(string Slot, BodyPartComponent Part);

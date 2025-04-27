using Content.Shared.Body.Part;

namespace Content.Server.AdventureSpace.Medical.Surgery.Events;

[ByRefEvent]
public readonly record struct SurgeryBodyPartAddedEvent(string Slot, BodyPartComponent Part);

[ByRefEvent]
public readonly record struct SurgeryBodyPartRemovedEvent(string Slot, BodyPartComponent Part);

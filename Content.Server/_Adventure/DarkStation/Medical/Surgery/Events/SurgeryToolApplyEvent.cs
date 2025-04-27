using Content.Shared.AdventureSpace.Medical.Surgery.Events.BodyParts;

namespace Content.Server.AdventureSpace.Medical.Surgery.Events;

public sealed class SurgeryToolApplyEvent(SurgeryBodyPartModel model) : HandledEntityEventArgs
{
    public SurgeryBodyPartModel Model = model;
}

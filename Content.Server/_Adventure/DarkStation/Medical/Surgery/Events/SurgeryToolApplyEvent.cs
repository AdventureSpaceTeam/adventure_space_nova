using Content.Shared._Adventure.Medical.Surgery.Events.BodyParts;

namespace Content.Server._Adventure.Medical.Surgery.Events;

public sealed class SurgeryToolApplyEvent(SurgeryBodyPartModel model) : HandledEntityEventArgs
{
    public SurgeryBodyPartModel Model = model;
}

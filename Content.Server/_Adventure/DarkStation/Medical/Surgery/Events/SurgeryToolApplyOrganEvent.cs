using Content.Shared._Adventure.Medical.Surgery.Events.Organs;

namespace Content.Server._Adventure.Medical.Surgery.Events;

public sealed class SurgeryToolApplyOrganEvent(SurgeryOrganModel model) : HandledEntityEventArgs
{
    public SurgeryOrganModel Model = model;
}

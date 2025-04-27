using Content.Shared.AdventureSpace.Medical.Surgery.Events.Organs;

namespace Content.Server.AdventureSpace.Medical.Surgery.Events;

public sealed class SurgeryToolApplyOrganEvent(SurgeryOrganModel model) : HandledEntityEventArgs
{
    public SurgeryOrganModel Model = model;
}

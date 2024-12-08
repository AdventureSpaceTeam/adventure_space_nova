using Content.Shared.AdventureSpace.Medical.Surgery.Events.Organs;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Events;

public sealed class SurgeryToolApplyOrganEvent(SurgeryOrganModel model) : HandledEntityEventArgs
{
    public SurgeryOrganModel Model = model;
}

using Content.Shared.Body.Organ;
using Robust.Shared.Serialization;

namespace Content.Shared.AdventurePrivate._Alteros.Medical.Surgery.Events.Organs;

[Serializable, NetSerializable]
public record SurgeryOrganModel(
    NetEntity User,
    NetEntity Target,
    NetEntity Tool,
    OrganSlot Slot
);

﻿using Content.Shared.Body.Part;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Events;

/**
 * Raised when tool removed from slot with body part
 */
public sealed class SurgeryToolRemovedFromBodyPart(BodyPartSlot slot) : HandledEntityEventArgs
{
    public BodyPartSlot Slot = slot;
}

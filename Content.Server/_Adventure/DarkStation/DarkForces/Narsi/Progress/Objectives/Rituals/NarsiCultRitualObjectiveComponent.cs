﻿using Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Buildings.Altar.Rituals.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Progress.Objectives.Rituals;

[RegisterComponent]
public sealed partial class NarsiCultRitualObjectiveComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public NarsiRitualPrototype? RequiredRitual;

    [DataField(required: true)]
    [ViewVariables(VVAccess.ReadWrite)]
    public List<ProtoId<NarsiRitualPrototype>> Rituals;
}

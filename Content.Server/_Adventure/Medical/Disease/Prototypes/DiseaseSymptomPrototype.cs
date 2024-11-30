﻿using Content.Server.AdventurePrivate._Alteros.Medical.Disease.Effects;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Disease.Prototypes;

[Prototype("symptom")]
public sealed class DiseaseSymptomPrototype : IPrototype
{
    [DataField(required: true)]
    public string Name { get; set; } = default!;

    [DataField(required: true)]
    public List<DiseaseEffect> Effects { get; set; } = default!;

    [IdDataField]
    public string ID { get; set; } = default!;
}

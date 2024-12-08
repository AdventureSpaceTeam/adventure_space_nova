using Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Tools;
using Content.Shared.Damage;

namespace Content.Server.AdventurePrivate._Alteros.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryDamageComponent : Component
{
    [DataField(required: true)]
    public Dictionary<SurgeryToolUsage, DamageSpecifier> Damage = [];
}

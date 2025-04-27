using Content.Server.AdventureSpace.Medical.Surgery.Tools;
using Content.Shared.Damage;

namespace Content.Server.AdventureSpace.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryDamageComponent : Component
{
    [DataField(required: true)]
    public Dictionary<SurgeryToolUsage, DamageSpecifier> Damage = [];
}

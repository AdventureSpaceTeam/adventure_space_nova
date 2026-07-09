using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Desecrated;

[RegisterComponent]
public sealed partial class DesecratedLightningComponent : Component
{
    [DataField(required: true, serverOnly: true)]
    public int FelDamage;

    [DataField(serverOnly: true)]
    public bool DoubleAttackConvert;
}

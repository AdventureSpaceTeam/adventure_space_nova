namespace Content.Server.AdventureSpace.DarkForces.Desecrated;

[RegisterComponent]
public sealed partial class DesecratedLightningComponent : Component
{
    [DataField(serverOnly: true)]
    public bool DoubleAttackConvert;

    [DataField(required: true, serverOnly: true)]
    public int FelDamage;
}

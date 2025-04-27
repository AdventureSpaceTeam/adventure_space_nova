namespace Content.Server._Adventure.GameRules.SCP.DClass;

[RegisterComponent]
[Access(typeof(DClassSystem))]
public sealed partial class DClassComponent : Component
{
    [DataField("pacifiedTime")]
    public TimeSpan PacifiedTime = TimeSpan.Zero;
}

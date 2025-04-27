using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.GameRules.SCP.SOAP;

[RegisterComponent]
public sealed partial class SCPSoapComponent : Component
{
    [DataField]
    public EntProtoId SlipAction = "SCPSoapSlipAction";

    [DataField]
    public float SlipActionForce = 15;

    [DataField]
    public float SlipActionRange = 1;

    [DataField]
    public SoundSpecifier SlipActionSound = new SoundPathSpecifier("/Audio/Effects/slip.ogg");

    [DataField]
    public float SlipActionStun = 4;

    [DataField]
    public EntityUid? SlipActionUid;
}

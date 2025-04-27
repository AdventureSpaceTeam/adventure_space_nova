using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.GameRules.SCP.SCP173;

[RegisterComponent]
[Access(typeof(SCP173System))]
public sealed partial class SCP173Component : Component
{
    [DataField]
    public EntProtoId BlindAction = "SCP173BlindAction";

    [DataField]
    public EntityUid? BlindActionUid;

    [DataField]
    public SoundSpecifier DoorOpenSound = default!;

    [DataField]
    public float EyeSightRange = 8;

    [DataField]
    public SoundSpecifier KillSoundCollection = default!;

    [ViewVariables(VVAccess.ReadOnly)]
    [DataField]
    public List<EntityUid> Lookers = new();

    [DataField]
    public SoundSpecifier ScaresSoundCollection = default!;

    [DataField]
    public EntProtoId ShartAction = "SCP173ShartAction";

    [DataField]
    public EntityUid? ShartActionUid;

    [DataField]
    public SoundSpecifier SpooksSoundCollection = default!;
}

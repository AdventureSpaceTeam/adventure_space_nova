using Robust.Shared.GameStates;

namespace Content.Shared._Adventure.DarkForces.Narsi.Cultist.Shadow;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class NarsiCultistShadowVisualizeComponent : Component
{

    [DataField("entityToCopy")]
    [AutoNetworkedField]
    public NetEntity EntityToCopy;
}

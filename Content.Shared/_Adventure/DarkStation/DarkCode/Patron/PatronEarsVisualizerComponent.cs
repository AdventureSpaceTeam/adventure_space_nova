using Robust.Shared.GameStates;

namespace Content.Shared._Adventure.Patron;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true)]
public sealed partial class PatronEarsVisualizerComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public string RsiPath = default!;
}

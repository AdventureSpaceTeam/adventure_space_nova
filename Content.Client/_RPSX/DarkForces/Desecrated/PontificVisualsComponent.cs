using Robust.Shared.GameObjects;

namespace Content.Client.RPSX.DarkForces.Desecrated;

[RegisterComponent]
public sealed partial class PontificVisualsComponent : Component
{

}

public enum PontificVisualLayers : byte
{
    Base,
    Dead,
    Flame,
    Prayer
}

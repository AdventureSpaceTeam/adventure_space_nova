using Content.Shared.Atmos;
using Robust.Shared.Player;

namespace Content.Server._Adventure.EnergyCores;

[RegisterComponent]
public sealed partial class HeatFreezingCoreComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string PortName { get; set; } = "pipe";

    [DataField]
    public HashSet<Gas> FilterGases = new()
    {
    };

    [DataField]
    public float FilterTemperature = Atmospherics.T0C + 470;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float MaxPressure = 3000;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float TransferRate = 100;

    [DataField]
    public Gas AbsorbGas = Gas.Frezon;

}

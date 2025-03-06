using Content.Shared.Alert;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Races.IPC;

/// <summary>
/// Код заимствован из BorgChassisComponent с удалением всего лишнего.  
/// В текущей версии компонент не имеет отличий от BorgChassisComponent.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedIPCSystem))]
public sealed partial class IPCComponent : Component
{
    [DataField]
    public ProtoId<AlertPrototype> BatteryAlert = "IPCBattery";

    [DataField]
    public ProtoId<AlertPrototype> NoBatteryAlert = "BorgBatteryNone";
}

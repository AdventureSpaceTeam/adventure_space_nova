using Content.Shared.DeviceLinking;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.EnergyCores;

[RegisterComponent]
public sealed partial class EnergyCoreConsoleComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? EnergyCoreEntity = null;

    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string LinkingPort = "EnergyCoreSender";

    [ViewVariables]
    public float EnergyCoreDamage = 0;
}
[Serializable, NetSerializable]
public enum EnergyCoreConsoleUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class EnergyCoreConsoleSelectedWattageMessage(float wattage) : BoundUserInterfaceMessage
{
    public float Wattage = wattage;
}

[Serializable, NetSerializable]
public sealed class EnergyCoreConsoleIsOnMessage(bool isOn) : BoundUserInterfaceMessage
{
    public bool IsOn = isOn;
}
[Serializable, NetSerializable]
public sealed class EnergyCoreConsoleTimeOfLifeMessage(float timeOfLife) : BoundUserInterfaceMessage
{
    public float TimeOfLife = timeOfLife;
}

[Serializable, NetSerializable]
public sealed class EnergyCoreConsoleUpdateState(
    NetEntity? energyCore,
    float timeOfLife,
    bool isOn,
    float curDamage
)
    : BoundUserInterfaceState
{
    public NetEntity? EnergyCore = energyCore;
    public float TimeOfLife = timeOfLife;
    public bool IsOn = isOn;
    public float CurDamage = curDamage;
}

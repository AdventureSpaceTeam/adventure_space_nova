using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Synth;

[Serializable, NetSerializable]
public enum SynthUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class SynthBuiState : BoundUserInterfaceState
{
    public float ChargePercent;

    public bool HasBattery;

    public SynthBuiState(float chargePercent, bool hasBattery)
    {
        ChargePercent = chargePercent;
        HasBattery = hasBattery;
    }
}

[Serializable, NetSerializable]
public sealed class SynthEjectBatteryBuiMessage : BoundUserInterfaceMessage
{

}
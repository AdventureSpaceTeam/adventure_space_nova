using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.DeviceLinking;
using Content.Shared.Radio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.GameObjects;

namespace Content.Shared._Adventure.EnergyCores;

[RegisterComponent]
[AutoGenerateComponentPause]
public sealed partial class EnergyCoreComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? EnergyCoreConsoleEntity = null;

    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string LinkingPort = "EnergyCoreReciever";

    [ViewVariables(VVAccess.ReadWrite)]
    public bool Working = true;

    [DataField]
    public string? OnState = "core_on";

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float SecPerMoles = 6;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextTick = TimeSpan.Zero;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public float TimeOfLife = 600;

    [ViewVariables(VVAccess.ReadWrite)]
    public bool ForceDisabled = false;

    [ViewVariables(VVAccess.ReadWrite)]
    public bool Overheat = false;

    [DataField(required: true)]
    public DamageSpecifier Damage = default!;

    [DataField]
    public float Heating = 50;

    [DataField]
    public float LifeAfterOverheat = -60;

    [DataField]
    public float CurrentPowerGeneration = 1;

    [DataField]
    public float EnablingLength = 3.6f;
    [DataField]
    public float DisablingLenght = 1.1f;

    [DataField(required: true)]
    public float BaseSupply = 100000;

    [DataField]
    public int Size = 1;

    [DataField]
    public bool isUndead = false;

    [DataField("engineeringChannel", customTypeSerializer: typeof(PrototypeIdSerializer<RadioChannelPrototype>))]
    public string EngineeringChannel = "Engineering";

    [DataField("lowTimeWarningSent")]
    public bool LowTimeWarningSent = false;

    [DataField("overheatSent")]
    public bool OverheatSent = false;
}

[Serializable, NetSerializable]
public enum EnergyCoreVisualLayers : byte
{
    IsOn,
    IsOff,
    Enabling,
    Disabling
}

[Serializable, NetSerializable]
public enum EnergyCoreState : byte
{
    Disabled,
    Enabled,
    Disabling,
    Enabling
}

[Serializable, NetSerializable]
public sealed partial class TogglePowerDoAfterEvent : SimpleDoAfterEvent
{
    public NetEntity? Initer;
    public TogglePowerDoAfterEvent(NetEntity? initer)
    {
        Initer = initer;
    }
}

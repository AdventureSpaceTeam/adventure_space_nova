using Content.Shared.Alert;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Synth.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedSynthSystem))]
public sealed partial class SynthComponent : Component
{
    [DataField]
    public ProtoId<AlertPrototype> BatteryAlert = "BorgBattery";

    [DataField]
    public ProtoId<AlertPrototype> NoBatteryAlert = "BorgBatteryNone";

    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier EmpDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            { "Shock", 10 },
        }
    };

    [DataField("empParalyzeTime")]
    public float EmpParalyzeTime = 10;

    [DataField]
    public EntProtoId DrainBatteryAction = "ActionDrainBattery";

    [DataField]
    public EntityUid? ActionEntity;

    public bool DrainActivated;
}

public sealed partial class ToggleDrainActionEvent : InstantActionEvent
{

}
using Content.Server._Adventure.Medical.Surgery.Organs.Components;
using Content.Server._Adventure.Medical.Surgery.Organs.Components.Base;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;

namespace Content.Server._Adventure.Medical.Surgery.Organs.Heart;

[RegisterComponent]
public sealed partial class CirculatoryPumpComponent : BaseOrganComponent, IIntervalOrgan
{
    //some mobs don't the er... the thing that makes you do and stuff
    [DataField]
    public bool Brainless;

    [DataField]
    public float DamageMod = 0.005f; //chance mod for a heart attack to occur every 5 seconds based on damage

    //consider minimum heart attack damage req
    [DataField]
    public float MinDamageThreshold = 2f; //minimum heart damage before heart attacks can occur

    [DataField]
    public DamageSpecifier NotWorkingDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            { "Asphyxiation", 30 },
        },
    };

    //if the player is overfed, strain is put on the pump
    //every update, existing strain has a chance to become permanent
    //the chance of damage becoming permanent and the permanent damage added are equal to the strain multiplied by the strain damage mod
    //the more strain, the greater the impact and risk
    [DataField]
    public float Strain; //if the player is overfed, put strain on the heart

    [DataField(required: true)]
    public float
        StrainCeiling; //strain ceiling for the period the player is overfed - strain is not increased unless it breaches this value

    [DataField(required: true)]
    public float
        StrainDamageMod; //mod for strain damage chance (% chance for strain to be permanent damage every 5 seconds, as well as the damage made permanent)

    [DataField(required: true)]
    public float StrainMod; //negation of overfed ceiling from strain (so that it does not start at 200)

    [DataField(required: true)]
    public float StrainRecovery; //amount of strain to recover every 5 seconds after player is no longer overfed

    [DataField]
    public TimeSpan NextCheckTime { get; set; }

    [DataField]
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(5);
}

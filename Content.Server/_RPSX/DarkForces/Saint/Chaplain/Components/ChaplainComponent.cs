using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Saint.Chaplain.Components;

[RegisterComponent]
public sealed partial class ChaplainComponent : Component
{
    [DataField]
    public EntProtoId NarsiExileAction = "ActionChaplainNarsiExile";

    [DataField]
    public EntityUid? NarsiExileActionEntity;

    [DataField]
    public EntProtoId GreatPrayerAction = "ActionChaplainGreatPrayer";

    [DataField]
    public EntityUid? GreatPrayerActionEntity;

    [DataField]
    public EntProtoId DefenceBarrierAction = "ActionChaplainDefenceBarrier";

    [DataField]
    public EntityUid? DefenceBarrierActionEntity;

    [DataField]
    public EntProtoId ExorcismAction = "ActionChaplainExorcism";

    [DataField]
    public EntityUid? ExorcismActionEntity;

    [DataField]
    public SoundSpecifier GreatPrayerSound = new SoundPathSpecifier("/Audio/DarkStation/DarkForces/Chaplain/great_prayer.ogg");

    [DataField]
    public EntityUid? GreatPrayerSoundEntity;

    [DataField]
    public DamageSpecifier FelHealDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            {"Fel", -20}
        }
    };
}

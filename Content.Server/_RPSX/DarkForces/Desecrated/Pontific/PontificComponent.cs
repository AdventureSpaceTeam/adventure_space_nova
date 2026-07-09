using System.Collections.Generic;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Desecrated.Pontific;

[RegisterComponent]
public sealed partial class PontificComponent : Component
{
    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public int PontificFel = 180;

    [DataField]
    [ViewVariables]
    public DamageSpecifier HealingDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            {"Blunt", -15},
            {"Slash", -15},
            {"Piercing", -15},
            {"Heat", -15},
            {"Cold", -15},
            {"Shock", -15},
            {"Holiness", -15}
        }
    };

    [DataField]
    public SoundSpecifier PrayerSound = new SoundPathSpecifier("/Audio/DarkStation/DarkForces/Pontific/pontific-prayer.ogg");

    [DataField]
    public List<EntProtoId> PontificActionsList = new()
    {
        "ActionPontificDarkPrayer",
        "ActionPontificLungeOfFaith",
        "ActionPontificBloodyAltar",
        "ActionPontificSpawnMonk",
        "ActionPontificSpawnGuardian",
        "ActionPontificFelLightning",
        "ActionPontificFlameSwords",
        "ActionPontificKudzu"
    };

    [DataField]
    public Dictionary<EntProtoId, EntityUid?> PontificActions = new ();

    [DataField]
    public ProtoId<AlertPrototype> PontificFelAlert = "PontificFel";
}

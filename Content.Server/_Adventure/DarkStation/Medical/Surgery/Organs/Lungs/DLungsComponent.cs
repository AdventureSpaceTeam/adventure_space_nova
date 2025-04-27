using Content.Server._Adventure.Medical.Surgery.Organs.Components.Base;

namespace Content.Server._Adventure.Medical.Surgery.Organs.Lungs;

[RegisterComponent]
public sealed partial class DLungsComponent : BaseOrganComponent
{
    [DataField]
    public List<string> DamageGases = ["Ammonia", "Tritium", "Plasma", "Frezon"];

    //reagent groups that can cause damage - default poisons and narcotics
    [DataField]
    public List<string> DamageGroups = ["Poison", "Narcotic"];

    //the modifier that sets the amount of damage done to the lungs whenever narcotics or toxins are inhaled
    //proportional to reagent metabolised
    [DataField]
    public float DamageMod = 0.0025f;

    [DataField]
    public List<string> IgnoreReagentsToxin = ["Nitrogen"];
}

namespace Content.Server._Adventure.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class SurgeryComponent : Component
{
    /**
     * Bleed
     */
    [DataField]
    public float BaseOrganBleed = 15f;

    [DataField]
    public float BasePartBleed = 20f;

    [DataField]
    public float BleedCheckInterval = 5f;

    [DataField]
    public float BleedLastChecked;


    //what inventory slots would block surgery (unless they have the surgeryGown component)
    public List<string> BlockingSlots = ["jumpsuit", "outerClothing"];

    /// <summary>
    ///     The species of organs compatible with the entity (other than its own species)
    ///     Part mismatches are tracked per part and deal damage periodically n number of times
    /// </summary>
    [DataField("compatibleSpecies", required: true)]
    public List<string> CompatibleSpecies = [];

    [DataField]
    public float InitialOrganBloodloss = 15f;

    [DataField]
    public float InitialPartBloodloss = 30f;

    /// <summary>
    ///     Update whenever a part or organ is removed, replaced, or whenever a clamp or cautery is used
    ///     Check all parts and determine if the owner is still bleeding
    ///     <see cref="SurgeryComponent" />
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool OrganBleeding = false;

    [ViewVariables(VVAccess.ReadWrite)]
    public bool PartBleeding = false;

    /// <summary>
    ///     Update whenever the owner is sedated or the sedation timer runs out
    ///     If a procedure takes places while the owner is not sedated, apply airloss to represent shock
    ///     Airloss applied should be based on relevant shock values multiplied by any time mods (a slow or improper procedure
    ///     will lead to more pain)
    ///     <see cref="SurgeryComponent" />
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Sedated = false;

    [ViewVariables(VVAccess.ReadOnly)]
    public float SurgeryBleed = 0f;
}

namespace Content.Server._Adventure.PlasMan.Components;

[RegisterComponent]
public sealed partial class SpontaneousCombustionComponent : Component
{
    /// <summary>
    /// </summary>
    [DataField]
    public float FireStacks = 1f;

    /// <summary>
    /// </summary>
    [DataField]
    public float MoleMinimum = 0.5f;

    /// <summary>
    /// </summary>
    [DataField]
    public string Gas = "Oxygen";

    /// <summary>
    /// </summary>
    [DataField("protectionSlots")]
    public List<string> ProtectionSlots = new() { "head", "outerClothing" };

    /// <summary>
    /// </summary>
    [DataField]
    public float CachedResistance = 1;
}

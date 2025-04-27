namespace Content.Shared._Adventure.Bank.Components;

[RegisterComponent]
public sealed partial class MarketModifierComponent : Component
{
    /// <summary>
    /// The amount to multiply a Static Price by
    /// </summary>
    [DataField(required: true)]
    public float Mod;
}

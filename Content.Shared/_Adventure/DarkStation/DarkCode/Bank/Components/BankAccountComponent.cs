namespace Content.Shared._Adventure.Bank.Components;

[RegisterComponent]
public sealed partial class BankAccountComponent : Component
{
    [DataField]
    public int Balance;
}

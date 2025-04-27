using Content.Shared._Adventure.Bank.Transactions;
using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Bank.PDA;

[Serializable, NetSerializable]
public sealed class BankCartridgeUiState : BoundUserInterfaceState
{
    public string Username;
    public int UserBalance;
    public List<BankTransaction> Transactions;

    public BankCartridgeUiState(string username, int userBalance, List<BankTransaction> transactions)
    {
        Username = username;
        UserBalance = userBalance;
        Transactions = transactions;
    }
}

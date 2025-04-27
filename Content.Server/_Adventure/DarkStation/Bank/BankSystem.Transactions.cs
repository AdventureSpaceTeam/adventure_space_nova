using Content.Shared.AdventureSpace.Bank.Transactions;

namespace Content.Server.AdventureSpace.Bank;

public sealed partial class BankSystem
{
    public BankTransaction CreateDepositTransaction(EntityUid source, int amount)
    {
        return new BankTransaction(
            MetaData(source).EntityName,
            BankTransactionType.Deposit,
            BankTransactionStatus.Success,
            BankBalanceChangeType.Income,
            amount
        );
    }

    public BankTransaction CreateSalaryTransaction(int amount, BankSalarySource source)
    {
        return new BankTransaction(
            $"{source}",
            BankTransactionType.Salary,
            BankTransactionStatus.Success,
            BankBalanceChangeType.Income,
            amount
        );
    }

    public BankTransaction CreateWithdrawTransaction(EntityUid source, int amount)
    {
        return new BankTransaction(
            MetaData(source).EntityName,
            BankTransactionType.Withdraw,
            BankTransactionStatus.Success,
            BankBalanceChangeType.Expense,
            amount
        );
    }

    public BankTransaction CreateBuyTransaction(EntityUid source, int amount)
    {
        return new BankTransaction(
            MetaData(source).EntityName,
            BankTransactionType.Buy,
            BankTransactionStatus.Success,
            BankBalanceChangeType.Expense,
            amount
        );
    }
}

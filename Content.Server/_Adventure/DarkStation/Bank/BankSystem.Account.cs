using System.Diagnostics.CodeAnalysis;
using Content.Server.GameTicking;
using Content.Shared._Adventure.Bank.Events;
using Content.Shared._Adventure.Bank.Transactions;
using Content.Shared.Preferences;
using Robust.Shared.Network;
using BankAccountComponent = Content.Shared._Adventure.Bank.Components.BankAccountComponent;

namespace Content.Server._Adventure.Bank;

public sealed partial class BankSystem : EntitySystem
{
    private void InitializeAccount()
    {
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawn);
        SubscribeLocalEvent<BankAccountComponent, BankExecuteTransactionEvent>(OnBankAccountChanged);
    }

    private void OnPlayerSpawn(PlayerSpawnCompleteEvent args)
    {
        if (!_mindSystem.TryGetMind(args.Mob, out var mindId, out var mind))
            return;

        var bankAccount = EnsureComp<BankAccountComponent>(mindId);
        bankAccount.Balance = args.Profile.BankBalance;
    }

    private async void OnBankAccountChanged(EntityUid mindId,
        BankAccountComponent bank,
        BankExecuteTransactionEvent args)
    {
        var transaction = args.Transaction;
        if (transaction.Amount <= 0)
        {
            args.Cancel();
            transaction.Status = BankTransactionStatus.Failure;
            UpdateRecordByTransaction(args.MobUid, transaction);
            return;
        }

        if (transaction.BalanceChangeType == BankBalanceChangeType.Expense && bank.Balance < transaction.Amount)
        {
            args.Cancel();
            transaction.Status = BankTransactionStatus.Failure;
            UpdateRecordByTransaction(args.MobUid, transaction);
            return;
        }

        if (!_prefsManager.TryGetCachedPreferences(args.UserId, out var prefs))
            return;

        var character = prefs.SelectedCharacter;
        var index = prefs.IndexOfCharacter(character);

        if (character is not HumanoidCharacterProfile profile)
            return;

        bank.Balance = GetBalanceByTransaction(bank, transaction);

        UpdateProfile(args.UserId, profile, index, bank.Balance);
        UpdateRecordByTransaction(args.MobUid, args.Transaction);
    }

    private async void UpdateProfile(NetUserId userId, HumanoidCharacterProfile profile, int index, int balance)
    {
        var newProfile = profile.WithBankBalance(balance);

        await _dbManager.SaveCharacterSlotAsync(userId, newProfile, index);
        _log.Info($"Character {profile.Name} saved");
    }

    private int GetBalanceByTransaction(BankAccountComponent bank, BankTransaction transaction)
    {
        if (transaction.BalanceChangeType == BankBalanceChangeType.Expense)
            return bank.Balance - transaction.Amount;
        return bank.Balance + transaction.Amount;
    }

    public bool TryExecuteTransaction(EntityUid uid, NetUserId netUid, BankTransaction transaction)
    {
        if (!_mindSystem.TryGetMind(uid, out var mindId, out _))
            return false;

        var ev = new BankExecuteTransactionEvent(uid, netUid, transaction);
        RaiseLocalEvent(mindId, ev);

        return !ev.Cancelled;
    }

    private bool TryGetBankAccount(EntityUid mobUid,
        [NotNullWhen(true)] out BankAccountComponent? bank,
        out EntityUid mindId)
    {
        bank = null;
        mindId = default;

        return _mindSystem.TryGetMind(mobUid, out mindId, out _) && TryComp(mindId, out bank);
    }

    public bool TryGetBankAccountBalance(EntityUid mobUid, out int balance)
    {
        balance = default;

        if (!TryGetBankAccount(mobUid, out var bank, out _))
            return false;

        balance = bank.Balance;
        return true;
    }
}

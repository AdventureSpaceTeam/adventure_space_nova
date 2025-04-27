using Content.Server._Adventure.Bank.Components;
using Content.Shared._Adventure.Bank;
using Content.Shared._Adventure.Bank.BUI;
using Content.Shared._Adventure.Bank.Events;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.Stacks;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using BankATMComponent = Content.Shared._Adventure.Bank.Components.BankATMComponent;

namespace Content.Server._Adventure.Bank;

public sealed partial class BankSystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string CashType = "SpaceCash";

    private void InitializeATM()
    {
        SubscribeLocalEvent<BankATMComponent, BankWithdrawMessage>(OnWithdraw);
        SubscribeLocalEvent<BankATMComponent, BankDepositMessage>(OnDeposit);
        SubscribeLocalEvent<BankATMComponent, BoundUIOpenedEvent>(OnATMUIOpen);
        SubscribeLocalEvent<BankATMComponent, EntInsertedIntoContainerMessage>(OnCashSlotChanged);
        SubscribeLocalEvent<BankATMComponent, EntRemovedFromContainerMessage>(OnCashSlotChanged);
    }

    private void OnWithdraw(EntityUid uid, BankATMComponent component, BankWithdrawMessage args)
    {
        if (!TryComp<ActorComponent>(args.Actor, out var actor))
            return;

        if (!TryGetBankAccount(args.Actor, out var bank, out _))
            return;

        if (!_uiSystem.HasUi(uid, args.UiKey))
            return;

        GetInsertedCashAmount(uid, out var deposit);

        if (bank.Balance < args.Amount)
        {
            PlayDenySound(uid, component, "bank-insufficient-funds");
            return;
        }

        // try to actually withdraw from the bank. Validation happens on the banking system but we still indicate error.
        var transaction = CreateWithdrawTransaction(uid, args.Amount);
        if (!TryExecuteTransaction(args.Actor, actor.PlayerSession.UserId, transaction))
        {
            PlayDenySound(uid, component, "bank-atm-menu-transaction-denied");
            return;
        }

        _adminLogger.Add(LogType.ATMUsage,
            LogImpact.Low,
            $"{ToPrettyString(args.Actor):actor} withdrew {args.Amount} from {ToPrettyString(uid)}");
        _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(bank.Balance, true, deposit));

        SpawnCash(uid, args.Amount);
        PlayConfirmSound(uid, component, "bank-atm-menu-withdraw-successful");
    }

    private void SpawnCash(EntityUid uid, int amount)
    {
        var stacks = _stackSystem.SpawnMultiple(CashType, amount, uid.ToCoordinates());
        foreach (var stack in stacks)
        {
            RemComp<BankSecureCashComponent>(stack);
        }
    }

    private void OnDeposit(EntityUid uid, BankATMComponent component, BankDepositMessage args)
    {
        if (!TryComp<ActorComponent>(args.Actor, out var actor))
            return;

        if (!TryGetBankAccount(args.Actor, out var bank, out _))
            return;

        if (!_uiSystem.HasUi(uid, args.UiKey))
            return;

        GetInsertedCashAmount(uid, out var deposit);

        var transaction = CreateDepositTransaction(uid, deposit);
        if (!TryExecuteTransaction(args.Actor, actor.PlayerSession.UserId, transaction))
        {
            PlayDenySound(uid, component, "bank-atm-menu-transaction-denied");
            return;
        }

        _adminLogger.Add(LogType.ATMUsage,
            LogImpact.Low,
            $"{ToPrettyString(args.Actor):actor} deposited {deposit} into {ToPrettyString(uid)}");
        _containerSystem.CleanContainer(_containerSystem.GetContainer(uid, BankATMComponent.CashSlotSlotId));
        _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(bank.Balance, true, 0));

        PlayConfirmSound(uid, component, "bank-atm-menu-deposit-successful");
    }

    private void OnCashSlotChanged(EntityUid uid, BankATMComponent component, ContainerModifiedMessage args)
    {
        var actor = _uiSystem.GetActors(uid, BankATMMenuUiKey.ATM).FirstOrNull();
        if (actor == null)
            return;

        GetInsertedCashAmount(uid, out var deposit);

        if (!TryGetBankAccount(actor.Value, out var bank, out _))
            return;

        _uiSystem.SetUiState(uid, BankATMMenuUiKey.ATM, new BankATMMenuInterfaceState(bank.Balance, true, deposit));
    }

    private void OnATMUIOpen(EntityUid uid, BankATMComponent component, BoundUIOpenedEvent args)
    {
        if (!TryGetBankAccount(args.Actor, out var bank, out _))
            return;

        if (!_uiSystem.HasUi(uid, args.UiKey))
            return;

        GetInsertedCashAmount(uid, out var deposit);
        _uiSystem.SetUiState(uid, args.UiKey, new BankATMMenuInterfaceState(bank.Balance, true, deposit));
    }

    private void GetInsertedCashAmount(EntityUid uid, out int amount)
    {
        if (!_itemSlots.TryGetSlot(uid, BankATMComponent.CashSlotSlotId, out var slot) ||
            !TryComp<StackComponent>(slot.Item, out var cashStack))
        {
            amount = 0;
            return;
        }

        amount = cashStack.Count;
    }

    private void PlayDenySound(EntityUid uid, BankATMComponent component, string message)
    {
        _audio.PlayPvs(_audio.GetSound(component.ErrorSound), uid);
        _popup.PopupEntity(Robust.Shared.Localization.Loc.GetString(message), uid, uid);
    }

    private void PlayConfirmSound(EntityUid uid, BankATMComponent component, string message)
    {
        _audio.PlayPvs(_audio.GetSound(component.ConfirmSound), uid);
        _popup.PopupEntity(Robust.Shared.Localization.Loc.GetString(message), uid, uid);
    }
}

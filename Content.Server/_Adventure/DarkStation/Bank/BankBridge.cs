using Content.Server._c4llv07e.Bridges;
using Content.Shared._Adventure.Bank.Transactions;
using Robust.Shared.Network;

namespace Content.Server._Adventure.Bank;

public sealed class BankBridge : IBankBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public BankTransaction CreateBuyTransaction(EntityUid uid, int price)
    {
        return _entityManager.System<BankSystem>().CreateBuyTransaction(uid, price);
    }

    public bool TryExecuteTransaction(EntityUid uid, NetUserId netUid, BankTransaction transaction)
    {
        return _entityManager.System<BankSystem>().TryExecuteTransaction(uid, netUid, transaction);
    }
}

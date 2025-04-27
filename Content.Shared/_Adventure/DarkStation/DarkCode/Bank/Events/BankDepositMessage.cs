using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.Bank.Events;

[Serializable, NetSerializable]

public sealed class BankDepositMessage : BoundUserInterfaceMessage
{
    // an empty message because we dont really want clients to be able to send funny ints to deposit
    public BankDepositMessage()
    {
    }
}

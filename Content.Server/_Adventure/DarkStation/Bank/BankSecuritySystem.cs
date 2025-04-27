using Content.Server.AdventureSpace.Bank.Components;
using Content.Server.Cargo.Components;
using Content.Shared.Examine;
using Content.Shared.Stacks;
using Robust.Shared.Configuration;

namespace Content.Server.AdventureSpace.Bank;

public sealed class BankSecuritySystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BankSecureCashComponent, StackSplitEvent>(OnStackSplitEvent);
        SubscribeLocalEvent<BankSecureCashComponent, StacksMergedEvent>(OnStackMerged);
        SubscribeLocalEvent<BankSecureCashComponent, StacksMergeAttemptEvent>(OnStacksMergeAttempt);
        SubscribeLocalEvent<BankSecureCashComponent, StacksAddContainerAttempt>(OnStacksAddContainerAttempt);
        SubscribeLocalEvent<BankSecureCashComponent, ExaminedEvent>(OnExaminedEvent);
        SubscribeLocalEvent<CargoSaleEvent>(OnCargoSaleEvent);
    }

    private void OnStacksAddContainerAttempt(EntityUid uid,
        BankSecureCashComponent component,
        StacksAddContainerAttempt args)
    {
        if (!_cfg.GetCVar(Shared.AdventureSpace.CCVars.SecretCCVars.EconomyEnabled))
            return;

        var donor = GetEntity(args.Donor);
        var recipient = GetEntity(args.Recipient);

        if (!CanStacksMerge(donor, recipient))
            args.Cancel();
    }

    private void OnStacksMergeAttempt(EntityUid uid, BankSecureCashComponent component, StacksMergeAttemptEvent args)
    {
        if (!_cfg.GetCVar(Shared.AdventureSpace.CCVars.SecretCCVars.EconomyEnabled))
            return;

        var donor = GetEntity(args.Donor);
        var recipient = GetEntity(args.Recipient);

        if (!CanStacksMerge(donor, recipient))
            args.Cancel();
    }

    private bool CanStacksMerge(EntityUid donor, EntityUid recipient)
    {
        if (HasComp<BankSecureCashComponent>(donor) && !HasComp<BankSecureCashComponent>(recipient))
            return false;

        if (HasComp<BankSecureCashComponent>(recipient) && !HasComp<BankSecureCashComponent>(donor))
            return false;

        return true;
    }

    private void OnExaminedEvent(EntityUid uid, BankSecureCashComponent component, ExaminedEvent args)
    {
        if (!_cfg.GetCVar(Shared.AdventureSpace.CCVars.SecretCCVars.EconomyEnabled))
            return;

        if (args.IsInDetailsRange)
            args.PushMarkup(Loc.GetString("bank-secure-cash-markup"));
    }

    private void OnStackMerged(EntityUid uid, BankSecureCashComponent component, StacksMergedEvent args)
    {
        if (!_cfg.GetCVar(Shared.AdventureSpace.CCVars.SecretCCVars.EconomyEnabled))
            return;

        if (HasComp<BankSecureCashComponent>(GetEntity(args.Donor)))
            EnsureComp<BankSecureCashComponent>(GetEntity(args.Recipient));
    }

    private void OnCargoSaleEvent(CargoSaleEvent args)
    {
        if (!_cfg.GetCVar(Shared.AdventureSpace.CCVars.SecretCCVars.EconomyEnabled))
            return;

        EnsureComp<BankSecureCashComponent>(args.CashStack);
    }

    private void OnStackSplitEvent(EntityUid uid, BankSecureCashComponent component, ref StackSplitEvent args)
    {
        if (!_cfg.GetCVar(Shared.AdventureSpace.CCVars.SecretCCVars.EconomyEnabled))
            return;

        EnsureComp<BankSecureCashComponent>(args.NewId);
    }
}

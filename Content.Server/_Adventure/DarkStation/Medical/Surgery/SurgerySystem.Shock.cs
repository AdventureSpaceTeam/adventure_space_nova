using System.Linq;
using Content.Server.AdventureSpace.Medical.Surgery.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Events;
using Content.Server.Body.Components;
using Content.Shared.Bed.Sleep;
using Content.Shared.Speech;

namespace Content.Server.AdventureSpace.Medical.Surgery;

public sealed partial class SurgerySystem
{
    private void InitializeSurgeryDamage()
    {
        SubscribeLocalEvent<SurgeryComponent, SurgeryToolAppliedEvent>(OnToolApplied);
    }

    private void OnToolApplied(EntityUid uid, SurgeryComponent component, SurgeryToolAppliedEvent args)
    {
        if (component.Sedated)
            return;

        if (!TryComp<SurgeryDamageComponent>(uid, out var surgeryDamage))
            return;

        var organs = GetAllBodyOrgans(uid);
        var hasBrain = organs.Any(HasComp<BrainComponent>);
        if (!hasBrain)
            return;

        if (!surgeryDamage.Damage.TryGetValue(args.ToolUsage, out var damage))
            return;

        _damageableSystem.TryChangeDamage(uid, damage, true);

        if (TryComp<SleepingComponent>(uid, out var sleeping))
            _sleepingSystem.TryWaking((uid, sleeping));

        var ev = new ScreamActionEvent();
        RaiseLocalEvent(uid, ev);
    }
}

using Content.Server.AdventureSpace.Medical.Surgery.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Standing;

namespace Content.Server.AdventureSpace.Medical.Surgery;

public sealed partial class SurgerySystem
{
    private void InitializeOpened()
    {
        SubscribeLocalEvent<SurgeryOpenedComponent, ComponentInit>(OnOpenedInit);
    }

    private void OnOpenedInit(EntityUid uid, SurgeryOpenedComponent component, ComponentInit args)
    {
        component.OpenedTick = _gameTiming.CurTime + component.OpenedThreshold;
    }

    private void UpdateOpened(TimeSpan curTime)
    {
        var query = EntityQueryEnumerator<SurgeryOpenedComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.OpenedTick > curTime)
                continue;

            if (!GetBodyFromPart(uid, out var body))
                continue;

            if (!TryComp<BuckleComponent>(body, out var buckle) || buckle.Buckled)
                continue;

            if (!TryComp<StandingStateComponent>(uid, out var standing) || !standing.Standing)
                continue;

            _damageableSystem.TryChangeDamage(uid, component.OpenedDamage, true);
            component.OpenedTick = curTime + component.OpenedThreshold;

            _popupSystem.PopupClient("Вы чувствуете не приятное тепло внутри тела", body.Value, body.Value);
        }
    }
}

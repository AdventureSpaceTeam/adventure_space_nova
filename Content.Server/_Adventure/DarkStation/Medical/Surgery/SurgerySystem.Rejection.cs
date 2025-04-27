using Content.Server._Adventure.Medical.Surgery.Components;
using Content.Server._Adventure.Medical.Surgery.Events;

namespace Content.Server._Adventure.Medical.Surgery;

public sealed partial class SurgerySystem
{
    private void InitializeRejection()
    {
        SubscribeLocalEvent<SurgeryRejectionComponent, ComponentInit>(OnRejectionInit);
        SubscribeLocalEvent<SurgeryRejectionComponent, SurgeryBodyPartRemovedEvent>(OnRejectionRemoved);
    }

    private void OnRejectionRemoved(EntityUid uid,
        SurgeryRejectionComponent component,
        SurgeryBodyPartRemovedEvent args)
    {
        RemComp<SurgeryRejectionComponent>(uid);
    }

    private void OnRejectionInit(EntityUid uid, SurgeryRejectionComponent component, ComponentInit args)
    {
        component.RejectionTick = _gameTiming.CurTime + component.RejectionThreshold;
    }

    private void UpdateRejection(TimeSpan curTime)
    {
        var query = EntityQueryEnumerator<SurgeryRejectionComponent>();
        while (query.MoveNext(out var uid, out var rejection))
        {
            if (rejection.RejectionTick > curTime || rejection.RejectionCounter >= rejection.RejectionRounds)
                continue;

            rejection.RejectionTick = curTime + rejection.RejectionThreshold;
            rejection.RejectionCounter++;

            if (!GetBodyFromPart(uid, out var body))
                continue;

            _damageableSystem.TryChangeDamage(body, rejection.RejectionDamage);
            _popupSystem.PopupEntity(Loc.GetString("surgery-warn-species-incompatible"), body.Value, body.Value);
        }
    }
}

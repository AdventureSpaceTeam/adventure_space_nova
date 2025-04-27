using Content.Server.AdventureSpace.Medical.Surgery.Organs.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Organs.Components.Base;
using Content.Server.AdventureSpace.Medical.Surgery.Organs.Events;
using Content.Server.Medical.Events;
using Content.Server.Popups;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Examine;
using Content.Shared.Mobs.Components;
using Content.Shared.Rejuvenate;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Content.Server.AdventureSpace.Medical.Surgery.Organs;

public abstract class SurgeryOrganSystem<T, TY> : EntitySystem
    where T : BaseOrganComponent
    where TY : Component
{
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly ISerializationManager _serialization = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<T, RejuvenateEvent>(OnRejuvenate);
        SubscribeLocalEvent<T, ExaminedEvent>(OnOrganExamined);
        SubscribeLocalEvent<T, OrganAddedToBodyEvent>(HandleAddedToBody);
        SubscribeLocalEvent<TY, OrganRemovedFromBodyEvent>(HandleRemovedFromBody);
        SubscribeLocalEvent<T, GetOrgansState>(OnGetOrgansState);
        SubscribeLocalEvent<T, SurgeryRequestOrganDamage>(OnRequestOrganDamage);
    }

    private void OnRequestOrganDamage(Entity<T> ent, ref SurgeryRequestOrganDamage args)
    {
        ent.Comp.Damage = args.Damage;
        UpdateOrganStatus(ent);
    }

    private void OnGetOrgansState(Entity<T> ent, ref GetOrgansState args)
    {
        var key = Loc.GetString($"organ-state-{ent.Comp.OrganKey}");
        args.OrgansState[key] = OrganUtils.GetOrganColoredState(ent.Comp.Condition);
    }

    private void OnRejuvenate(Entity<T> ent, ref RejuvenateEvent args)
    {
        ent.Comp.Working = true;
        ent.Comp.Condition = OrganCondition.Good;
        ent.Comp.Damage = 0;

        OnRejuvenate(ent);
    }

    protected virtual void OnRejuvenate(Entity<T> ent)
    {
    }

    private void HandleAddedToBody(Entity<T> ent, ref OrganAddedToBodyEvent args)
    {
        // In some reason, player already has this comp
        if (HasComp<T>(args.Body))
            Log.Warning($"In Some Reason {ent} already has {ent.Comp} component on added to body event!");

        if (!TryComp<T>(ent, out var donorComp))
        {
            Log.Error($"Donor component {typeof(T)} not found in {ent}");
            return;
        }

        var donorEnt = (ent.Owner, donorComp);
        var newEnt = CopyComponent(donorEnt, args.Body);
        if (!donorEnt.donorComp.Embedded)
            RemComp(donorEnt.Owner, donorComp);

        MarkWithComponent(ent);
        OnAddedToBody(newEnt, donorEnt);
    }

    private void HandleRemovedFromBody(Entity<TY> ent, ref OrganRemovedFromBodyEvent args)
    {
        if (!TryComp<T>(args.OldBody, out var comp))
            return;

        var newEnt = CopyComponent((args.OldBody, comp), ent);
        if (!comp.Embedded)
            RemCompDeferred<T>(args.OldBody);

        UnMarkWithComponent(newEnt);
        OnRemovedFromBody(newEnt, (args.OldBody, comp));
    }

    private Entity<T> CopyComponent(Entity<T> oldEnt, EntityUid newEnt)
    {
        var newComponent = (Component)_componentFactory.GetComponent(typeof(T));
        newComponent.Owner = newEnt;

        var temp = (object)newComponent;
        _serialization.CopyTo(oldEnt.Comp, ref temp);

        AddComp(newEnt, (Component)temp!, true);
        return (newEnt, (T)temp!);
    }

    private void MarkWithComponent(Entity<T> ent)
    {
        var newComponent = (Component)_componentFactory.GetComponent(typeof(TY));
        newComponent.Owner = ent;

        AddComp(ent, newComponent);
    }

    private void UnMarkWithComponent(Entity<T> ent)
    {
        RemComp<TY>(ent);
    }

    protected virtual void OnAddedToBody(Entity<T> ent, Entity<T> oldEnt)
    {
    }

    protected void OnRemovedFromBody(Entity<T> ent, Entity<T> oldEnt)
    {
    }

    private void OnOrganExamined(Entity<T> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange || HasComp<MobStateComponent>(ent))
            return;

        args.PushMarkup($"Состояние органа: {OrganUtils.GetOrganColoredState(ent.Comp.Condition)}");
    }

    public override void Update(float frameTime)
    {
        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<T>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp is not IIntervalOrgan organ)
                break;

            if (organ.NextCheckTime > curTime)
                continue;

            organ.NextCheckTime = curTime + organ.CheckInterval;

            var ent = (uid, comp);
            if (organ is BaseRegeneratableOrganComponent regenOrgan)
            {
                RegenerateOrgan((uid, regenOrgan));
                UpdateOrganStatus(ent);
            }

            OnOrganInterval(ent);
        }
    }

    protected virtual void RegenerateOrgan(Entity<BaseRegeneratableOrganComponent> ent)
    {
        if (!ent.Comp.Working || ent.Comp.Damage <= 0)
            return;

        var delta = ent.Comp.Damage - ent.Comp.RegenerationAmount;
        ent.Comp.Damage = delta < 0 ? 0 : delta;
    }

    protected virtual void OnOrganInterval(Entity<T> ent)
    {
    }

    protected void UpdateOrganStatus(Entity<T> ent)
    {
        var oldCondition = ent.Comp.Condition;
        CheckOrganStatus(ent);

        if (oldCondition != ent.Comp.Condition)
            SendOrganStateMessage(ent);
    }

    private void SendOrganStateMessage(Entity<T> ent)
    {
        var loc = Loc.GetString($"{ent.Comp.OrganKey}-condition-{ent.Comp.Condition.ToString().ToLower()}");
        _popupSystem.PopupEntity(loc, ent, ent);
    }

    private void CheckOrganStatus(Entity<T> ent)
    {
        if (ent.Comp.Damage >= ent.Comp.MaxDamageThreshold)
            ent.Comp.Condition = OrganCondition.Failure;
        else if (ent.Comp.Damage >= ent.Comp.CriticalThreshold)
            ent.Comp.Condition = OrganCondition.Critical;
        else if (ent.Comp.Damage >= ent.Comp.WarningThreshold)
            ent.Comp.Condition = OrganCondition.Warning;
        else
            ent.Comp.Condition = OrganCondition.Good;
    }

    protected void SetFailure(Entity<T> ent)
    {
        if (!ent.Comp.Working)
            return;

        ent.Comp.Working = false;
        ent.Comp.Condition = OrganCondition.Failure;

        SendOrganStateMessage(ent);
    }
}

using Content.Server.AdventureSpace.Medical.Disease.Diagnosis.Diagnoser.Components;
using Content.Server.AdventureSpace.Medical.Disease.Diagnosis.Swab;
using Content.Server.AdventureSpace.Medical.Disease.Systems;
using Content.Server.Popups;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.AdventureSpace.Medical.Disease.Diagnosis.Diagnoser;

public sealed partial class DiseaseDiagnoserSystem : EntitySystem
{
    [Dependency] private readonly DiseaseSystem _disease = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ItemSlotsSystem _slots = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DiseaseDiagnoserComponent, ContainerIsInsertingAttemptEvent>(OnInsertAttempt);
        SubscribeLocalEvent<DiseaseDiagnoserComponent, EntInsertedIntoContainerMessage>(OnInserted);

        SubscribeLocalEvent<DiseaseDiagnoserComponent, GetVerbsEvent<Verb>>(OnGetVerbs);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _gameTiming.CurTime;
        var query = EntityQueryEnumerator<DiseaseDiagnoserComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.State == DiseaseDiagnoserState.IDLE)
                continue;

            if (component.DiagnoserFinishedTime > curTime)
                continue;

            OnDiagnoserFinished((uid, component));
        }
    }

    private void OnDiagnoserFinished(Entity<DiseaseDiagnoserComponent> diagnoser)
    {
        PrintDiseases(diagnoser);

        var component = diagnoser.Comp;
        component.State = DiseaseDiagnoserState.IDLE;

        _slots.SetLock(diagnoser, component.SwabSlotId, false);
        _popupSystem.PopupEntity(Loc.GetString("diagnoser-finished"), diagnoser);
    }

    #region Control

    private void OnInserted(EntityUid uid, DiseaseDiagnoserComponent component, EntInsertedIntoContainerMessage args)
    {
        if (args.Container.ID != component.SwabSlotId)
            return;

        _popupSystem.PopupEntity(Loc.GetString("diagnoser-ready"), uid);
    }

    private void OnInsertAttempt(EntityUid uid,
        DiseaseDiagnoserComponent component,
        ContainerIsInsertingAttemptEvent args)
    {
        if (args.Container.ID != component.SwabSlotId)
            return;

        if (HasComp<DiseaseSwabComponent>(args.EntityUid))
            return;

        args.Cancel();
    }

    private void OnGetVerbs(EntityUid uid, DiseaseDiagnoserComponent component, GetVerbsEvent<Verb> args)
    {
        if (args.Hands == null || !args.CanInteract)
            return;

        AddDiagnoserControlVerbs((uid, component), args);
    }

    private void AddDiagnoserControlVerbs(Entity<DiseaseDiagnoserComponent> diagnoser, GetVerbsEvent<Verb> args)
    {
        Verb? verb = null;

        switch (diagnoser.Comp.State)
        {
            case DiseaseDiagnoserState.IDLE:
                if (_slots.TryGetSlot(diagnoser, diagnoser.Comp.SwabSlotId, out var itemSlot) && itemSlot.Item != null)
                {
                    verb = new Verb
                    {
                        Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/Structures/Machines/diagnoser.rsi"),
                            "icon"),
                        Text = Loc.GetString("diagnoser-verb-start-analyze"),
                        Act = () => StartDiagnoser(diagnoser),
                    };
                }

                break;

            case DiseaseDiagnoserState.RUNNING:
                verb = new Verb
                {
                    Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/Structures/Machines/diagnoser.rsi"), "icon"),
                    Text = Loc.GetString("diagnoser-verb-stop-analyze"),
                    Act = () => StopDiagnoser(diagnoser),
                };
                break;
        }

        if (verb == null)
            return;

        args.Verbs.Add(verb);
    }

    private void StartDiagnoser(Entity<DiseaseDiagnoserComponent> diagnoser)
    {
        var component = diagnoser.Comp;

        component.State = DiseaseDiagnoserState.RUNNING;
        component.DiagnoserFinishedTime = _gameTiming.CurTime + component.DiagnoserFinishedThreshold;

        _slots.SetLock(diagnoser, component.SwabSlotId, true);
        _popupSystem.PopupEntity(Loc.GetString("diagnoser-started"), diagnoser);
    }

    private void StopDiagnoser(Entity<DiseaseDiagnoserComponent> diagnoser)
    {
        var component = diagnoser.Comp;

        component.State = DiseaseDiagnoserState.IDLE;

        _slots.SetLock(diagnoser, component.SwabSlotId, false);
        _popupSystem.PopupEntity(Loc.GetString("diagnoser-stopped"), diagnoser);
    }

    #endregion Control
}

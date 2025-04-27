using System.Linq;
using Content.Server.AdventureSpace.Medical.Disease.Diagnosis.Diagnoser.Components;
using Content.Server.AdventureSpace.Medical.Disease.Prototypes;
using Content.Server.AdventureSpace.Medical.Disease.Systems;
using Content.Server.Popups;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Paper;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.AdventureSpace.Medical.Disease.Diagnosis.Vaccinator;

public sealed class DiseaseVaccinatorSystem : EntitySystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string VaccinatorReportPaper = "VaccinatorReportPaper";

    [Dependency] private readonly DiseaseSystem _disease = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ItemSlotsSystem _slots = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DiseaseVaccinatorComponent, ContainerIsInsertingAttemptEvent>(OnInsertAttempt);
        SubscribeLocalEvent<DiseaseVaccinatorComponent, EntInsertedIntoContainerMessage>(OnInserted);
        SubscribeLocalEvent<DiseaseVaccinatorComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<DiseaseVaccinatorComponent, GetVerbsEvent<Verb>>(OnGetVerb);
    }

    private void OnGetVerb(EntityUid uid, DiseaseVaccinatorComponent component, GetVerbsEvent<Verb> args)
    {
        if (component.State != VaccinatorState.WAITING_DISEASE)
            return;

        if (!_slots.TryGetSlot(uid, component.PaperSlotId, out var paperSlot) || paperSlot.Item is not { } paper)
            return;

        if (!TryComp<DiseaseDiagnoserPaperComponent>(paper, out var diseaseDiagnoserPaper))
            return;

        foreach (var prototype in diseaseDiagnoserPaper.DiseasesPrototypes)
        {
            if (!_prototypeManager.TryIndex<DiseasePrototype>(prototype, out var diseasePrototype))
                continue;

            var verb = new Verb
            {
                Text = diseasePrototype.Name,
                Act = () => OnDiseaseSelected((uid, component), prototype),
            };

            args.Verbs.Add(verb);
        }
    }

    private void OnDiseaseSelected(Entity<DiseaseVaccinatorComponent> vaccinator, string prototype)
    {
        vaccinator.Comp.SelectedDisease = prototype;
        CalculateReagents(vaccinator);
    }

    private void OnExamined(EntityUid uid, DiseaseVaccinatorComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        using (args.PushGroup(nameof(DiseaseVaccinatorComponent)))
        {
            switch (component.State)
            {
                case VaccinatorState.WAITING_REAGENTS:
                {
                    args.PushMarkup("[color=red]Вакцинатор ожидает бутылочки со следующими реагентами:[/color]");

                    foreach (var reagent in component.RequiredReagents)
                    {
                        if (!_prototypeManager.TryIndex<ReagentPrototype>(reagent, out var reagentPrototype))
                        {
                            args.PushMarkup("[color=yellow]- Неизвестный реагент[/color]");
                            continue;
                        }

                        args.PushMarkup($"[color=green]-{reagentPrototype.LocalizedName}[/color]");
                    }

                    return;
                }
                case VaccinatorState.IDLE:
                    args.PushMarkup("[color=green]Вакцинатор ожидает результат диагностировщика[/color]");
                    return;
                case VaccinatorState.WAITING_DISEASE:
                    args.PushMarkup("[color=yellow]Вакцинатор ожидает выбор болезни[/color]");
                    return;
            }
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var curTime = _gameTiming.CurTime;
        var query = EntityQueryEnumerator<DiseaseVaccinatorComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.State != VaccinatorState.RUNNING)
                continue;

            if (component.VaccinatorFinishedTime > curTime)
                continue;

            OnVaccinatorFinished((uid, component));
        }
    }

    private void OnVaccinatorFinished(Entity<DiseaseVaccinatorComponent> ent)
    {
        SpawnVaccinatorResult(ent);
        SetIdleState(ent);
        _popupSystem.PopupEntity("Анализ завершен", ent);
    }

    private void SetIdleState(Entity<DiseaseVaccinatorComponent> ent)
    {
        SetSlotsLock(ent, false, ClearBottle);

        if (_slots.TryEject(ent, ent.Comp.PaperSlotId, null, out var paper))
            QueueDel(paper);

        ent.Comp.State = VaccinatorState.IDLE;
        ent.Comp.SelectedDisease = string.Empty;
        ent.Comp.RequiredReagents.Clear();
    }

    private void SpawnVaccinatorResult(Entity<DiseaseVaccinatorComponent> ent)
    {
        var result = Spawn(VaccinatorReportPaper, Transform(ent).Coordinates);

        if (!TryComp<PaperComponent>(result, out var paperResult))
            return;

        var paperContent = "Лекарства от болезни является реагент:";
        var cureReagents = _disease.GetDiseaseCureReagents(ent.Comp.SelectedDisease);

        foreach (var reagent in cureReagents)
        {
            if (_prototypeManager.TryIndex(reagent, out var reagentPrototype))
                continue;

            paperContent += $"\n - {reagentPrototype?.LocalizedName}";
        }

        paperResult.Content = paperContent;
    }

    private void ClearBottle(EntityUid bottle)
    {
        if (!_solutionContainerSystem.TryGetSolution(bottle, "drink", out var solutionEntity))
            return;

        var solution = solutionEntity.Value.Comp.Solution;

        solution.Contents.Clear();
        solution.Volume = 0f;

        _solutionContainerSystem.UpdateChemicals(solutionEntity.Value, false);
    }

    private void OnInserted(Entity<DiseaseVaccinatorComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        switch (ent.Comp.State)
        {
            case VaccinatorState.IDLE:
                OnInsertedOnIdle(ent, args);
                break;
            case VaccinatorState.WAITING_REAGENTS:
                OnInsertedOnReagents(ent, args);
                break;
        }
    }

    private void OnInsertedOnIdle(Entity<DiseaseVaccinatorComponent> ent, EntInsertedIntoContainerMessage args)
    {
        if (args.Container.ID != ent.Comp.PaperSlotId)
            return;

        OnPaperInserted(ent, args.Entity);
    }

    private void OnInsertedOnReagents(Entity<DiseaseVaccinatorComponent> ent, EntInsertedIntoContainerMessage args)
    {
        if (!ent.Comp.ReagentsSlotIds.Contains(args.Container.ID))
            return;

        CheckReagentsSlots(ent);
    }

    private void CheckReagentsSlots(Entity<DiseaseVaccinatorComponent> ent)
    {
        var validateMap = new Dictionary<string, bool>();

        foreach (var slotId in ent.Comp.ReagentsSlotIds)
        {
            if (!_slots.TryGetSlot(ent, slotId, out var slotItem))
                return;

            if (slotItem.Item is not { } item)
                return;

            if (!_solutionContainerSystem.TryGetSolution(item, "drink", out var solution))
                return;


            if (solution.Value.Comp.Solution.Contents.Count > 1)
                return;

            foreach (var requiredReagent in ent.Comp.RequiredReagents)
            {
                if (!solution.Value.Comp.Solution.ContainsPrototype(requiredReagent))
                    continue;

                validateMap[requiredReagent] = true;
            }
        }

        if (validateMap.Values.Any(result => !result))
            return;

        ent.Comp.State = VaccinatorState.RUNNING;
        ent.Comp.VaccinatorFinishedTime = _gameTiming.CurTime + ent.Comp.VaccinatorFinishedThreshold;

        SetSlotsLock(ent, true);
        _popupSystem.PopupEntity("Началось изучение...", ent);
    }

    private void OnPaperInserted(Entity<DiseaseVaccinatorComponent> vaccinator, EntityUid paper)
    {
        if (!TryComp<DiseaseDiagnoserPaperComponent>(paper, out var paperComp))
            return;

        switch (paperComp.DiseasesPrototypes.Count)
        {
            case > 1:
                vaccinator.Comp.State = VaccinatorState.WAITING_DISEASE;
                _popupSystem.PopupEntity("Выберите болезнь", vaccinator);

                return;
            case 0:
                vaccinator.Comp.State = VaccinatorState.IDLE;
                _popupSystem.PopupEntity("Не найдены болезни", vaccinator);

                return;
            default:
                vaccinator.Comp.SelectedDisease = paperComp.DiseasesPrototypes.First();
                break;
        }

        CalculateReagents(vaccinator);
    }

    private void CalculateReagents(Entity<DiseaseVaccinatorComponent> vaccinator)
    {
        var diseaseId = vaccinator.Comp.SelectedDisease;
        if (!_disease.CanCureDisease(diseaseId))
        {
            vaccinator.Comp.SelectedDisease = string.Empty;
            vaccinator.Comp.State = VaccinatorState.IDLE;

            _popupSystem.PopupEntity("Лекарство от болезни не может быть синтезировано", vaccinator);

            return;
        }

        var targetReagents = _disease.GetDiseaseVaccinatorReagents(diseaseId);

        vaccinator.Comp.State = VaccinatorState.WAITING_REAGENTS;
        vaccinator.Comp.RequiredReagents.AddRange(targetReagents);

        foreach (var slotId in vaccinator.Comp.ReagentsSlotIds)
        {
            _slots.SetLock(vaccinator, slotId, false);
        }

        _popupSystem.PopupEntity("Поместите бутылочки с указанными реагентами", vaccinator);
    }

    private void OnInsertAttempt(Entity<DiseaseVaccinatorComponent> ent, ref ContainerIsInsertingAttemptEvent args)
    {
        if (args.Container.ID == ent.Comp.PaperSlotId)
            return;

        if (!ent.Comp.ReagentsSlotIds.Contains(args.Container.ID))
            return;

        if (_slots.TryGetSlot(ent, ent.Comp.PaperSlotId, out var paperSlot) && paperSlot.Item != null)
            return;

        _popupSystem.PopupEntity("Сначала поместите документ с анализом болезни", ent);
        args.Cancel();
    }

    private void SetSlotsLock(Entity<DiseaseVaccinatorComponent> ent, bool locked, Action<EntityUid>? action = null)
    {
        foreach (var slotId in ent.Comp.ReagentsSlotIds)
        {
            if (_slots.TryGetSlot(ent, slotId, out var itemSlot) && itemSlot.Item is { } item)
                action?.Invoke(item);

            _slots.SetLock(ent, slotId, locked);
        }

        _slots.SetLock(ent, ent.Comp.PaperSlotId, locked);
    }
}

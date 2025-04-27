using Content.Shared._Adventure.Roles.StationAI.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Random.Helpers;
using Content.Shared.Silicons.Laws;
using Content.Shared.Silicons.Laws.Components;

namespace Content.Server._Adventure.Roles.StationAI;

public sealed partial class StationAISystem
{
    private void InitializeGhost()
    {
        SubscribeLocalEvent<StationAIGhostComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<StationAIGhostComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<StationAIGhostComponent, GetSiliconLawsEvent>(OnGetLaws);
        SubscribeLocalEvent<StationAIGhostComponent, CannotRichMessageAttemptEvent>(OnCannotRichMessageAttempt);
    }

    private void OnCannotRichMessageAttempt(EntityUid uid,
        StationAIGhostComponent component,
        CannotRichMessageAttemptEvent args)
    {
        args.Cancel();
    }

    private void OnComponentInit(EntityUid uid, StationAIGhostComponent component, ComponentInit args)
    {
        _actionsSystem.AddAction(uid, ref component.DoorOpenEntity, component.DoorOpen, uid);
        _actionsSystem.AddAction(uid, ref component.DoorBoltEntity, component.DoorBolt, uid);
        _actionsSystem.AddAction(uid, ref component.DoorEmergencyEntity, component.DoorEmergency, uid);
        _actionsSystem.AddAction(uid, ref component.DoorElectrifyEntity, component.DoorElectrify, uid);
        _actionsSystem.AddAction(uid, ref component.AIVisitEntity, component.AIVisitAction, uid);
    }

    private void OnComponentShutdown(EntityUid uid, StationAIGhostComponent component, ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(uid, component.DoorOpenEntity);
        _actionsSystem.RemoveAction(uid, component.DoorBoltEntity);
        _actionsSystem.RemoveAction(uid, component.DoorEmergencyEntity);
        _actionsSystem.RemoveAction(uid, component.DoorElectrifyEntity);
        _actionsSystem.RemoveAction(uid, component.AIVisitEntity);
    }

    private void OnGetLaws(Entity<StationAIGhostComponent> ent, ref GetSiliconLawsEvent args)
    {
        if (ent.Comp.SelectedLaw == null)
        {
            var selectedLaw = _prototypeManager.Index(ent.Comp.LawsId).Pick();
            if (_prototypeManager.TryIndex<SiliconLawsetPrototype>(selectedLaw, out var newLaw))
                ent.Comp.SelectedLaw = newLaw;
        }

        if (ent.Comp.SelectedLaw == null)
            return;

        foreach (var law in ent.Comp.SelectedLaw.Laws)
        {
            args.Laws.Laws.Add(_prototypeManager.Index<SiliconLawPrototype>(law));
        }

        args.Handled = true;
    }
}

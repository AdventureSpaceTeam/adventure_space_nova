using Content.Shared.AdventureSpace.Roles.StationAI.Components;
using Content.Shared.AdventureSpace.Roles.StationAI.Events;
using Content.Shared.Destructible;

namespace Content.Server.AdventureSpace.Roles.StationAI;

public sealed partial class StationAISystem
{
    private void InitializeVisitor()
    {
        SubscribeLocalEvent<StationAIVisitorComponent, ComponentInit>(OnVisitorInit);
        SubscribeLocalEvent<StationAIVisitorComponent, ComponentShutdown>(OnVisitorShutdown);
        SubscribeLocalEvent<StationAIVisitorComponent, DestructionEventArgs>(OnVisitorDestroy);
        SubscribeLocalEvent<StationAIVisitorComponent, StationAIBackToBodyEvent>(OnVisitorBackToBody);

        SubscribeLocalEvent<StationAIGhostComponent, StationAIControlEvent>(OnControlEvent);
    }

    private void OnVisitorInit(EntityUid uid, StationAIVisitorComponent component, ComponentInit args)
    {
        _actionsSystem.AddAction(uid, ref component.ActionBackToAIGhost, component.BackToAIGhostAction);
    }

    private void OnVisitorShutdown(EntityUid uid, StationAIVisitorComponent component, ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(uid, component.ActionBackToAIGhost);
    }

    private void OnVisitorDestroy(EntityUid uid, StationAIVisitorComponent component, DestructionEventArgs args)
    {
        DropVisitingByVisitor((uid, component));
    }

    private void OnVisitorBackToBody(EntityUid uid, StationAIVisitorComponent component, StationAIBackToBodyEvent args)
    {
        DropVisitingByVisitor((uid, component));
    }

    private void DropVisitingByVisitor(Entity<StationAIVisitorComponent> aiVisitor)
    {
        if (aiVisitor.Comp.AIGhost == null || !_mindSystem.TryGetMind(aiVisitor, out var mindId, out _))
            return;

        _mindSystem.TransferTo(mindId, aiVisitor.Comp.AIGhost);
        RemComp<StationAIVisitorComponent>(aiVisitor);
    }

    private void OnControlEvent(EntityUid uid, StationAIGhostComponent component, StationAIControlEvent args)
    {
        if (!_mindSystem.TryGetMind(uid, out var mindId, out _))
            return;

        var aiVisitorComp = EnsureComp<StationAIVisitorComponent>(args.Target);
        aiVisitorComp.AIGhost = uid;

        _mindSystem.TransferTo(mindId, args.Target);
        component.VisitingEntity = args.Target;
    }

    private void DropVisiting(Entity<StationAIGhostComponent> aiGhost)
    {
        if (aiGhost.Comp.VisitingEntity == EntityUid.Invalid)
            return;

        if (!_mindSystem.TryGetMind(aiGhost.Comp.VisitingEntity, out var mindId, out _))
            return;

        _mindSystem.TransferTo(mindId, aiGhost);
        RemComp<StationAIVisitorComponent>(aiGhost.Comp.VisitingEntity);
    }
}

using Content.Server._Adventure.DarkForces.Narsi.Progress.Components;
using Content.Shared.Objectives.Components;

namespace Content.Server._Adventure.DarkForces.Narsi.Progress.Objectives;

public sealed class NarsiObjectiveSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NarsiObjectiveComponent, ObjectiveGetProgressEvent>(OnGetProgress);
    }

    private void OnGetProgress(EntityUid uid, NarsiObjectiveComponent component, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = component.Completed ? 1f : 0f;
    }
}

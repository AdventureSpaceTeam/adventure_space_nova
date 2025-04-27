using Content.Server._Adventure.Medical.Surgery.Components;
using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Shared._Adventure.Medical.Surgery.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Part;
using Content.Shared.Humanoid;
using Robust.Shared.Containers;

namespace Content.Server._Adventure.Medical.Surgery.Body;

public sealed partial class SurgeryBodySystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SurgeryComponent, OrganAddedToBodyEvent>(OnOrganAdded);
        SubscribeLocalEvent<SurgeryComponent, OrganRemovedFromBodyEvent>(OnOrganRemoved);

        SubscribeLocalEvent<SurgeryComponent, BodyPartAddedEvent>(OnBodyPartAdded);
        SubscribeLocalEvent<SurgeryComponent, BodyPartRemovedEvent>(OnBodyPartRemoved);

        SubscribeLocalEvent<SurgeryBodyPartComponent, BodyPartInitializeInBodyEvent>(OnPartFirstlyAddedToBody);
    }

    private void OnOrganRemoved(Entity<SurgeryComponent> ent, ref OrganRemovedFromBodyEvent args)
    {
        //Nothing
    }

    private void OnOrganAdded(Entity<SurgeryComponent> ent, ref OrganAddedToBodyEvent args)
    {
        //Nothing
    }

    private void OnBodyPartAdded(Entity<SurgeryComponent> ent, ref BodyPartAddedEvent args)
    {
        var partUid = args.Part.Owner;
        var partRelayEv = new SurgeryBodyPartAddedEvent(args.Slot, args.Part);
        RaiseLocalEvent(partUid, ref partRelayEv);

        var updatedEv = new SurgeryBodyUpdated(ent);
        RaiseLocalEvent(ent, ref updatedEv);

        OnPartAddedToBody(ent, ref args);
    }

    private void OnBodyPartRemoved(Entity<SurgeryComponent> ent, ref BodyPartRemovedEvent args)
    {
        var partUid = args.Part.Owner;
        var partRelayEv = new SurgeryBodyPartRemovedEvent(args.Slot, args.Part);
        RaiseLocalEvent(partUid, ref partRelayEv);

        var updatedEv = new SurgeryBodyUpdated(ent);
        RaiseLocalEvent(ent, ref updatedEv);

        if (!TryComp<HumanoidAppearanceComponent>(ent, out var humanoid))
            return;

        BodyPartFirstInit((partUid, args.Part), (ent, humanoid));
    }

    private void OnPartAddedToBody(Entity<SurgeryComponent> ent, ref BodyPartAddedEvent args)
    {
        if (!TryComp(ent, out HumanoidAppearanceComponent? humanoid))
            return;

        BodyPartApplyMarking((args.Part.Owner, args.Part), (ent, humanoid));
        Dirty(ent, humanoid);
    }

    private void OnPartFirstlyAddedToBody(EntityUid uid,
        SurgeryBodyPartComponent component,
        BodyPartInitializeInBodyEvent args)
    {
        if (!TryComp(args.Body, out HumanoidAppearanceComponent? humanoid))
            return;

        component.Visuals.Color = humanoid.SkinColor;
    }
}

using Content.Server._Adventure.Medical.Surgery.Components;
using Content.Server._Adventure.Medical.Surgery.Tools.Components;
using Content.Server.Body.Systems;
using Content.Shared._Adventure.Medical.Surgery.Components;
using Content.Shared._Adventure.Medical.Surgery.Events;
using Content.Shared._Adventure.Medical.Surgery.Events.BodyParts;
using Content.Shared._Adventure.Medical.Surgery.Events.Organs;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.DoAfter;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Popups;
using Robust.Server.Audio;
using Robust.Shared.Containers;

namespace Content.Server._Adventure.Medical.Surgery.Tools;

public sealed partial class SurgeryToolsSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly BodySystem _bodySystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedHandsSystem _handsSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly SurgerySystem _surgerySystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SurgeryToolComponent, SurgeryRemoveToolDoAfter>(OnRemoveToolFromPart);
        SubscribeLocalEvent<SurgeryToolComponent, SurgeryRemoveToolSlotDoAfter>(OnRemoveToolFromSlot);
        SubscribeLocalEvent<SurgeryToolComponent, SurgeryRemoveToolOrganDoAfter>(OnRemoveToolFromOrgan);

        InitializeCauterize();
        InitializeIncisor();
        InitializeDrill();
        InitializeClamp();

        InitializeRetractor();
        InitializeSaw();
        InitializeSmallClamp();
        InitializeSuture();
        InitializeHardSuture();
    }

    private void PlaySoundWithPopup(EntityUid user, EntityUid tool, string loc)
    {
        if (!TryComp<SurgeryToolComponent>(tool, out var toolComponent))
            return;

        _audioSystem.PlayPvs(toolComponent.Sound, user);
        _popupSystem.PopupEntity(Loc.GetString(loc), user, user);
    }

    private void UpdateTarget(EntityUid? target, SurgeryToolUsage toolUsage)
    {
        if (target == null)
            return;

        _surgerySystem.SetBodyStatusFromChange(target.Value, toolUsage);
        _surgerySystem.UpdateUiState(target.Value);
    }

    private void UpdatePartStatus(Entity<SurgeryBodyPartComponent> entity)
    {
        if (!entity.Comp.Container)
            return;

        HandleBodyOpened(entity);

        var state = entity.Comp.State;
        if (state is { ExoSkeleton: true, ExoOpened: false }
            or { Incisable: true, Incised: false }
            or { EndoSkeleton: true, EndoOpened: false })
        {
            entity.Comp.State.Opened = false;
            return;
        }

        if (!BodyPartHasAttachment<SurgeryRetractorComponent>(entity) && state.Incisable)
            return;

        //Это используется для слота
        entity.Comp.State.Opened = true;
    }

    private void HandleBodyOpened(Entity<SurgeryBodyPartComponent> entity)
    {
        var state = entity.Comp.State;
        if (state is { ExoOpened: true } or { Incised: true } or { EndoOpened: true })
        {
            EnsureComp<SurgeryOpenedComponent>(entity);
            return;
        }

        RemComp<SurgeryOpenedComponent>(entity);
    }

    private void StartDoAfter(DoAfterEvent @event, SurgeryBodyPartModel model)
    {
        var tool = GetEntity(model.Tool);
        var user = GetEntity(model.User);
        var target = GetEntity(model.Target);

        if (!TryComp<SurgeryToolComponent>(tool, out var toolComponent))
            return;

        StartDoAfter(@event, user, target, (tool, toolComponent));
    }

    private void StartDoAfter(DoAfterEvent @event, SurgeryOrganModel model)
    {
        var tool = GetEntity(model.Tool);
        var user = GetEntity(model.User);
        var target = GetEntity(model.Target);

        if (!TryComp<SurgeryToolComponent>(tool, out var toolComponent))
            return;

        StartDoAfter(@event, user, target, (tool, toolComponent));
    }

    private void StartDoAfter(DoAfterEvent @event, EntityUid user, EntityUid target, Entity<SurgeryToolComponent> tool)
    {
        var doAfterEvent = new DoAfterArgs(
            EntityManager,
            user,
            TimeSpan.FromSeconds(tool.Comp.ApplyTime),
            @event,
            tool,
            target,
            tool
        )
        {
            BreakOnDamage = true,
            BreakOnHandChange = true,
            BreakOnMove = true,
            DuplicateCondition = DuplicateConditions.SameTool,
        };

        _doAfterSystem.TryStartDoAfter(doAfterEvent);
    }

    /**
     * Containers
     */
    private void RemoveToolFrom(NetEntity netEntity, EntityUid tool)
    {
        var target = GetEntity(netEntity);
        var attachmentContainer = _containerSystem.EnsureContainer<Container>(target, "slotAttachment");
        _containerSystem.Remove(tool, attachmentContainer);
    }

    private void InsertToolTo(NetEntity netEntity, EntityUid tool)
    {
        var target = GetEntity(netEntity);
        var attachmentContainer = _containerSystem.EnsureContainer<Container>(target, "slotAttachment");
        _containerSystem.Insert(tool, attachmentContainer);
    }

    /**
     * Utils
     */
    public bool OrganSlotHasAttachment<T>(OrganSlot slot) where T : Component
    {
        return HasComp<T>(GetEntity(slot.Attachment));
    }

    public bool BodyPartSlotHasAttachment<T>(BodyPartSlot slot) where T : Component
    {
        return HasComp<T>(GetEntity(slot.Attachment));
    }

    public bool BodyPartHasAttachment<T>(SurgeryBodyPartComponent part) where T : Component
    {
        return part.State.Attachment is not null && HasComp<T>(GetEntity(part.State.Attachment));
    }
}

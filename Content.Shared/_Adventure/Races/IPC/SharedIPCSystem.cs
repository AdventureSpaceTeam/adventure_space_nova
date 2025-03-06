using Content.Shared.IdentityManagement;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;

namespace Content.Shared._Adventure.Races.IPC;

/// <summary>
/// This handles logic, interactions, and UI related to <see cref="IPCComponent"/> and other related components.
/// Код заимствован из SharedBorgSystem с удалением всего лишнего.  
/// В текущей версии компонент не имеет отличий от SharedBorgSystem.
/// </summary>
public abstract partial class SharedIPCSystem : EntitySystem
{
    [Dependency] protected readonly ItemToggleSystem Toggle = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();


        SubscribeLocalEvent<IPCComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeedModifiers);
        SubscribeLocalEvent<TryGetIdentityShortInfoEvent>(OnTryGetIdentityShortInfo);
    }

    private void OnTryGetIdentityShortInfo(TryGetIdentityShortInfoEvent args)
    {
        if (args.Handled)
        {
            return;
        }

        if (!HasComp<IPCComponent>(args.ForActor))
        {
            return;
        }

        args.Title = Name(args.ForActor).Trim();
        args.Handled = true;
    }

    private void OnRefreshMovementSpeedModifiers(EntityUid uid, IPCComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        if (Toggle.IsActivated(uid))
            return;

        if (!TryComp<MovementSpeedModifierComponent>(uid, out var movement))
            return;

        var sprintDif = movement.BaseWalkSpeed / movement.BaseSprintSpeed;
        args.ModifySpeed(1f, sprintDif);
    }
}

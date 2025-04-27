using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared._Adventure.Roles.StationAI.Components;
using Content.Shared.Speech;

namespace Content.Server._Adventure.Roles.StationAI;

public sealed partial class StationAISystem
{
    [Dependency] private readonly BatterySystem _batterySystem = default!;

    private void InitializeElectric()
    {
        SubscribeLocalEvent<StationAICoreComponent, ChargeChangedEvent>(OnPowerChangedEvent);
        SubscribeLocalEvent<StationAIGhostComponent, SpeakAttemptEvent>(OnSpeakAttempt);
    }

    private void OnSpeakAttempt(EntityUid uid, StationAIGhostComponent component, SpeakAttemptEvent args)
    {
        if (component.CoreUid == EntityUid.Invalid)
            return;

        if (!TryComp<StationAICoreComponent>(component.CoreUid, out var aiCoreComponent) || aiCoreComponent.Enabled)
            return;

        args.Cancel();
    }

    private void OnPowerChangedEvent(EntityUid uid, StationAICoreComponent component, ChargeChangedEvent args)
    {
        var isPowered = args.Charge > 0f;
        if (isPowered == component.Enabled)
            return;

        var ghostCore = component.GhostUid;
        if (ghostCore == EntityUid.Invalid || !TryComp<StationAIGhostComponent>(ghostCore, out var ghostComponent))
            return;

        var actions = _actionsSystem.GetActions(ghostCore);
        foreach (var action in actions)
        {
            _actionsSystem.SetEnabled(action.Id, isPowered);
        }

        component.Enabled = isPowered;

        if (isPowered)
            return;

        DropCamera((ghostCore, ghostComponent));
        DropVisiting((ghostCore, ghostComponent));
        _userInterfaceSystem.CloseUis(ghostCore);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<StationAICoreComponent, BatteryComponent>();
        while (query.MoveNext(out var uid, out var stationAI, out _))
        {
            if (!stationAI.Enabled)
                continue;

            _batterySystem.UseCharge(uid, stationAI.Wattage);
            UpdateBatteryState(uid, stationAI.GhostUid);
        }
    }

    private void UpdateBatteryState(EntityUid core, EntityUid ghost)
    {
        if (!TryComp<BatteryComponent>(core, out var battery) || ghost == EntityUid.Invalid)
            return;

        if (!TryComp<StationAIGhostComponent>(ghost, out var ghostComp))
            return;

        var chargePercent = (short)MathF.Round(battery.CurrentCharge / battery.MaxCharge * 10f);

        _alerts.ClearAlert(ghost, ghostComp.BorgBatteryNone);
        _alerts.ShowAlert(ghost, ghostComp.BorgBattery, chargePercent);
    }

    private bool IsCoreEnabled(Entity<StationAIGhostComponent> aiGhost)
    {
        return TryComp<StationAICoreComponent>(aiGhost.Comp.CoreUid, out var coreComponent) && coreComponent.Enabled;
    }
}

using Content.Server.Doors.Systems;
using Content.Server.Electrocution;
using Content.Server.Power.Components;
using Content.Shared.AdventureSpace.Roles.StationAI.Components;
using Content.Shared.AdventureSpace.Roles.StationAI.Events;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction.Events;

namespace Content.Server.AdventureSpace.Roles.StationAI;

public sealed partial class StationAISystem
{
    [Dependency] private readonly AirlockSystem _airlock = default!;
    [Dependency] private readonly DoorSystem _doorSystem = default!;

    private void InitializeDoors()
    {
        SubscribeLocalEvent<StationAIGhostComponent, StationAIDoorBolt>(OnDoorBolt);
        SubscribeLocalEvent<StationAIGhostComponent, StationAIDoorOpenClose>(OnDoorOpenClose);
        SubscribeLocalEvent<StationAIGhostComponent, StationAIDoorEmergencyAccess>(OnDoorEmergencyAccess);
        SubscribeLocalEvent<StationAIGhostComponent, StationAIDoorElectrify>(OnDoorElectrify);

        SubscribeLocalEvent<StationAIGhostComponent, CtrlUseInWorldEvent>(OnUseCtrlInWorld);
        SubscribeLocalEvent<StationAIGhostComponent, AltUseInWorldEvent>(OnUseAltInWorld);
    }

    private void OnDoorEmergencyAccess(EntityUid uid,
        StationAIGhostComponent component,
        StationAIDoorEmergencyAccess args)
    {
        if (!TryComp<AirlockComponent>(args.Target, out var airlockComp))
            return;

        _airlock.ToggleEmergencyAccess(args.Target, airlockComp);
        _adminLogger.Add(LogType.Action,
            LogImpact.Medium,
            $"AI {ToPrettyString(uid):player} set emergency access {(airlockComp.EmergencyAccess ? "on" : "off")}");
    }

    private void OnDoorOpenClose(EntityUid uid, StationAIGhostComponent component, StationAIDoorOpenClose args)
    {
        if (!TryComp<DoorComponent>(args.Target, out var doorComp))
            return;

        if (_doorSystem.TryToggleDoor(args.Target, doorComp, uid))
        {
            _adminLogger.Add(LogType.Action,
                LogImpact.Medium,
                $"AI {ToPrettyString(uid):player} change door state {ToPrettyString(args.Target)}: {doorComp.State}");
        }
    }

    private void OnDoorBolt(EntityUid uid, StationAIGhostComponent component, StationAIDoorBolt args)
    {
        ToggleDoorBolt(uid, component, args.Target);
    }

    private void OnUseAltInWorld(EntityUid uid, StationAIGhostComponent component, AltUseInWorldEvent args)
    {
        if (!IsCoreEnabled((uid, component)))
            return;

        ToggleDoorBolt(uid, component, args.Target);
    }

    private void ToggleDoorBolt(EntityUid uid, StationAIGhostComponent component, EntityUid target)
    {
        if (!TryComp<DoorBoltComponent>(target, out var boltsComp) ||
            !TryComp<ApcPowerReceiverComponent>(target, out var powerConsumer))
            return;

        if (boltsComp.BoltWireCut || powerConsumer is { NeedsPower: true, Powered: false })
            return;

        _doorSystem.SetBoltsDown((target, boltsComp), !boltsComp.BoltsDown);
        _adminLogger.Add(LogType.Action,
            LogImpact.Medium,
            $"AI {ToPrettyString(uid):player} change bolt state {ToPrettyString(target)} to {(boltsComp.BoltsDown ? "" : "un")}bolt it");
    }

    private void OnUseCtrlInWorld(EntityUid uid, StationAIGhostComponent component, CtrlUseInWorldEvent args)
    {
        if (!HasComp<DoorComponent>(args.Target) || !IsCoreEnabled((uid, component)))
            return;

        ToggleElectrify(uid, component, args.Target);
    }

    private void OnDoorElectrify(EntityUid uid, StationAIGhostComponent component, StationAIDoorElectrify args)
    {
        ToggleElectrify(uid, component, args.Target);
    }

    private void ToggleElectrify(EntityUid uid, StationAIGhostComponent component, EntityUid target)
    {
        if (!TryComp<ElectrifiedComponent>(target, out var electrifiedComponent))
            return;

        electrifiedComponent.Enabled = !electrifiedComponent.Enabled;
        _adminLogger.Add(LogType.Action,
            LogImpact.Medium,
            $"AI {ToPrettyString(uid):player} changed electrify enabled of door - {ToPrettyString(target)} to {electrifiedComponent.Enabled}");
    }
}

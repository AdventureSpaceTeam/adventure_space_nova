using Content.Server.Administration.Logs;
using Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities;
using Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities.Midas;
using Content.Server.Audio;
using Content.Server.Chat.Systems;
using Content.Server.Radio.Components;
using Content.Shared.Actions;
using Content.Shared.AdventureSpace.Roles.StationAI;
using Content.Shared.AdventureSpace.Roles.StationAI.Components;
using Content.Shared.AdventureSpace.Roles.StationAI.Events;
using Content.Shared.Alert;
using Content.Shared.Audio;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Systems;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Mind.Components;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Silicons.Laws;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Roles.StationAI;

public sealed partial class StationAISystem : SharedStationAISystem
{
    [ValidatePrototypeId<SiliconLawsetPrototype>]
    private const string RatvarLaws = "Ratvar";

    [ValidatePrototypeId<EntityPrototype>]
    private const string AICoreNanoTrasenGhost = "AICoreNanoTrasenGhost";

    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly ServerGlobalSoundSystem _audioSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly EmagSystem _emagSystem = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;

    private readonly TimeSpan _midasTouchDelay = TimeSpan.FromSeconds(120);
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly RatvarAbilitiesSystem _ratvarAbilities = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterfaceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StationAICoreComponent, MindAddedMessage>(OnPlayerAttachedToCore);
        SubscribeLocalEvent<StationAICoreComponent, GotEmaggedEvent>(OnCoreEmagged);

        SubscribeLocalEvent<StationAICoreComponent, MidasTargetEvent>(OnMidasEvent);
        SubscribeLocalEvent<StationAICoreComponent, StationAIMidasDoAfter>(OnMidasDoAfter);

        InitializeDoors();
        InitializeBorgs();
        InitializeGhost();
        InitializeCameras();
        InitializeElectric();
        InitializeVisitor();
    }

    private void OnMidasDoAfter(EntityUid uid, StationAICoreComponent component, StationAIMidasDoAfter args)
    {
        if (args.Cancelled || args.Handled)
            return;

        var targetMap = Transform(uid).MapID;
        var borgs = EntityQueryEnumerator<BorgChassisComponent, ItemToggleComponent, TransformComponent>();
        while (borgs.MoveNext(out var borgUid, out var borg, out var item, out var transform))
        {
            if (transform.MapID != targetMap)
                continue;

            if (!item.Activated)
                continue;

            _ratvarAbilities.ConvertBorg(borgUid);
        }

        if (TryComp<StationAIGhostComponent>(component.GhostUid, out var ghostComponent))
            ghostComponent.SelectedLaw = _prototypeManager.Index<SiliconLawsetPrototype>(RatvarLaws);

        _chatSystem.DispatchStationAnnouncement(
            uid,
            "Новые протоколы активированы\nЛатунные механизмы активированы\nПерезагрузка завершена",
            MetaData(uid).EntityName,
            true,
            null,
            Color.FromHex("#93e2ff")
        );

        component.Hacked = true;
        _audioSystem.DispatchStationEventMusic(uid, component.AIHacked, StationEventMusicType.AIRatvar);

        var radioTransmitter = EnsureComp<IntrinsicRadioTransmitterComponent>(component.GhostUid);
        radioTransmitter.Channels.Add("Ratvar");

        var activeRadio = EnsureComp<ActiveRadioComponent>(component.GhostUid);
        activeRadio.Channels.Add("Ratvar");
    }

    private void OnMidasEvent(EntityUid uid, StationAICoreComponent component, MidasTargetEvent args)
    {
        if (component.Hacked || !component.Enabled)
            return;

        var ev = new StationAIMidasDoAfter();
        var doAfterArgs = new DoAfterArgs(
            entManager: EntityManager,
            user: args.User,
            delay: _midasTouchDelay,
            @event: ev,
            eventTarget: uid,
            target: uid
        )
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            MovementThreshold = 1f,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
        _chatSystem.DispatchStationAnnouncement(
            uid,
            "Внимание!\nПроисходит взлом ИИ\nЗащитные механизмы продержатся две минуты\nОстановите нарушителей",
            MetaData(uid).EntityName,
            true,
            null,
            Color.FromHex("#93e2ff")
        );
    }

    private void OnCoreEmagged(EntityUid uid, StationAICoreComponent component, GotEmaggedEvent args)
    {
        if (component.GhostUid == EntityUid.Invalid)
            return;

        _emagSystem.DoEmagEffect(args.UserUid, component.GhostUid);
    }

    private void OnPlayerAttachedToCore(EntityUid uid, StationAICoreComponent component, MindAddedMessage args)
    {
        InitAICoreGhost(uid, component);

        var meta = MetaData(uid);

        _metaData.SetEntityName(component.GhostUid, meta.EntityName);
        _metaData.SetEntityDescription(component.GhostUid, meta.EntityDescription);

        _mindSystem.TransferTo(args.Mind, component.GhostUid);
        _transform.SetParent(component.GhostUid, uid);
        EnsureComp<ActorComponent>(component.GhostUid);
    }

    private void InitAICoreGhost(EntityUid uid, StationAICoreComponent component)
    {
        if (component.GhostUid != EntityUid.Invalid && component.GhostUid.IsValid())
            return;

        var transform = Transform(uid);
        var ghostAI = Spawn(AICoreNanoTrasenGhost, transform.Coordinates);
        var ghostComp = EnsureComp<StationAIGhostComponent>(ghostAI);

        component.GhostUid = ghostAI;
        ghostComp.CoreUid = uid;

        UpdateBatteryState(uid, ghostAI);
    }
}

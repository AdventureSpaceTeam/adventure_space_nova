using System.Collections.Generic;
using Content.Server.Antag;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.RoundEnd;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.GameTicking.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.RPSX.DarkForces.Desecrated;
using Content.Shared.Station.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Content.Server.RPSX.GameTicking.Rules.Pontific;

public sealed class PontificRuleSystem : GameRuleSystem<PontificRuleComponent>
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string MobPontific = "MobPontific";

    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly RoundEndSystem _roundEnd = default!;
    [Dependency] private readonly SharedTransformSystem _sharedTransform = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public float EndRoundFriction = 0.75f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PontificRuleComponent, AntagSelectLocationEvent>(OnSelectPontificLocation);
        SubscribeLocalEvent<PontificRuleComponent, AntagSelectEntityEvent>(OnSelectPontificEntity);
    }

    private void OnSelectPontificEntity(EntityUid uid, PontificRuleComponent component, ref AntagSelectEntityEvent args)
    {
        args.Entity = Spawn(MobPontific);
    }

    private void OnSelectPontificLocation(EntityUid uid, PontificRuleComponent component,
        ref AntagSelectLocationEvent args)
    {
        if (!TryFindRandomTile(out _, out _, out _, out var coordinates))
            return;

        args.Coordinates = new List<MapCoordinates> { coordinates.ToMap(EntityManager, _sharedTransform) };
    }

    protected override void ActiveTick(EntityUid uid, PontificRuleComponent component, GameRuleComponent gameRule,
        float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);
        if (_timing.CurTime < component.NextRoundEndCheck)
            return;

        component.NextRoundEndCheck += component.EndCheckDelay;
        CheckRoundEnd(uid, component, gameRule);
    }

    private void CheckRoundEnd(EntityUid uid, PontificRuleComponent comp, GameRuleComponent gameRule)
    {
        if (!GameTicker.IsGameRuleActive(uid, gameRule))
            return;


        if (!comp.ShuttleCalled && GetInfectedFraction(false) >= comp.DeathShuttleCallPercentage)
        {
            comp.ShuttleCalled = true;
            foreach (var station in _station.GetStations())
            {
                _chat.DispatchStationAnnouncement(station, Loc.GetString("pontific-rule-auto-evac"),
                    colorOverride: Color.Crimson);
            }

            _roundEnd.RequestRoundEnd(null, false);
        }

        if (GetInfectedFraction() >= EndRoundFriction)
            _roundEnd.EndRound();
    }

    private List<EntityUid> GetHealthyHumans(bool includeOffStation = true)
    {
        var healthy = new List<EntityUid>();

        var stationGrids = new HashSet<EntityUid>();
        if (!includeOffStation)
        {
            foreach (var station in _station.GetStationsSet())
            {
                if (TryComp<StationDataComponent>(station, out var data) && _station.GetLargestGrid((station, data)) is { } grid)
                    stationGrids.Add(grid);
            }
        }

        var players =
            AllEntityQuery<HumanoidAppearanceComponent, ActorComponent, MobStateComponent, TransformComponent>();
        var desecrated = GetEntityQuery<DesecratedMarkerComponent>();
        while (players.MoveNext(out var uid, out _, out _, out var mob, out var xform))
        {
            if (!_mobState.IsAlive(uid, mob))
                continue;

            if (desecrated.HasComponent(uid))
                continue;

            if (!includeOffStation && !stationGrids.Contains(xform.GridUid ?? EntityUid.Invalid))
                continue;

            healthy.Add(uid);
        }

        return healthy;
    }

    private float GetInfectedFraction(bool includeOffStation = true, bool includeDead = false)
    {
        var players = GetHealthyHumans(includeOffStation);
        var desecrateCount = 0;
        var query = EntityQueryEnumerator<HumanoidAppearanceComponent, DesecratedMarkerComponent, MobStateComponent>();
        while (query.MoveNext(out _, out _, out _, out var mob))
        {
            if (!includeDead && mob.CurrentState == MobState.Dead)
                continue;
            desecrateCount++;
        }

        return desecrateCount / (float) (players.Count + desecrateCount);
    }
}

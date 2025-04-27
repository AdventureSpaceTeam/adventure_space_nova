using System.Linq;
using Content.Server.AdventureSpace.GameRules.SCP.Station.Components;
using Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Events;
using Content.Server.AdventureSpace.GameRules.SCP.Station.Goals.Prototypes;
using Content.Server.AdventureSpace.Utils;
using Content.Server.Chat.Systems;
using Content.Server.Fax;
using Content.Shared.Fax.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Paper;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Content.Server.AdventureSpace.GameRules.SCP.Station.Goals;

public sealed class SCPGoalSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly IComponentFactory _factory = default!;
    [Dependency] private readonly FaxSystem _faxSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ISerializationManager _serialization = default!;

    private List<SCPStationGoalPrototype> _goalPrototypes = [];

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCPStationGoalComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SCPGoalComponent, ComponentInit>(OnSCPGoalInit);
        SubscribeLocalEvent<SCPGoalComponent, SCPGoalComplete>(OnSCPGoalComplete);
        SubscribeLocalEvent<SCPGoalComponent, EntParentChangedMessage>(OnParentChanged);
        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnProtoReloaded);

        LoadGoalsPrototypes();
    }

    private void OnParentChanged(Entity<SCPGoalComponent> ent, ref EntParentChangedMessage args)
    {
        ent.Comp.LocatedAtMainStation = StationUtils.IsEntityOnMainStationOnly(ent, EntityManager);
    }

    private void OnSCPGoalInit(Entity<SCPGoalComponent> ent, ref ComponentInit args)
    {
        ent.Comp.LocatedAtMainStation = StationUtils.IsEntityOnMainStationOnly(ent, EntityManager);
    }

    private void OnSCPGoalComplete(Entity<SCPGoalComponent> ent, ref SCPGoalComplete args)
    {
        var name = MetaData(ent).EntityName;
        _chatSystem.DispatchGlobalAnnouncement(
            $"Цель с SCP {name} выполнена!",
            "Центральное командование",
            true
        );

        foreach (var type in ent.Comp.Components)
        {
            RemComp(ent, type);
        }

        RemComp<SCPGoalComponent>(ent);
    }

    private void OnComponentInit(Entity<SCPStationGoalComponent> ent, ref ComponentInit args)
    {
        ent.Comp.NextTaskDelay = _gameTiming.CurTime + ent.Comp.NextTaskThreshold;
    }

    private void OnProtoReloaded(PrototypesReloadedEventArgs ev)
    {
        if (!ev.WasModified<SCPStationGoalPrototype>())
            return;

        LoadGoalsPrototypes();
    }

    private void LoadGoalsPrototypes()
    {
        _goalPrototypes = _prototypeManager
            .EnumeratePrototypes<SCPStationGoalPrototype>()
            .ToList();
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<SCPStationGoalComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.NextTaskDelay > _gameTiming.CurTime)
                continue;

            UpdateGoals((uid, component));
        }
    }

    private void UpdateGoals(Entity<SCPStationGoalComponent> ent)
    {
        _random.Shuffle(_goalPrototypes);

        var query = EntityQueryEnumerator<SCPMarkerComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (HasComp<SCPGoalComponent>(uid))
                continue;

            if (_mobState.IsDead(uid))
                continue;

            var scpPrototype = MetaData(uid).EntityPrototype;
            if (scpPrototype == null)
                continue;

            var goalPrototype = _random.Pick(_goalPrototypes);
            if (goalPrototype.Prototypes != null && goalPrototype.Prototypes.Contains(scpPrototype.ID))
                continue;

            ent.Comp.NextTaskDelay = _gameTiming.CurTime + ent.Comp.NextTaskThreshold;
            SetupGoalComponents((uid, component), goalPrototype);
            SendGoalMessages(ent, uid, goalPrototype);
            return;
        }

        ent.Comp.NextTaskDelay = _gameTiming.CurTime + ent.Comp.NextTaskThreshold;
    }

    private void SetupGoalComponents(Entity<SCPMarkerComponent> ent, SCPStationGoalPrototype goalPrototype)
    {
        var scpGoalComponent = EnsureComp<SCPGoalComponent>(ent);
        scpGoalComponent.Components = [];

        foreach (var (name, data) in goalPrototype.Components)
        {
            var component = (Component)_factory.GetComponent(name);
            component.Owner = ent;

            var temp = (object)component;
            _serialization.CopyTo(data.Component, ref temp);
            RemComp(ent, temp!.GetType());
            AddComp(ent, (Component)temp);

            scpGoalComponent.Components.Add(temp.GetType());
        }
    }

    private void SendGoalMessages(Entity<SCPStationGoalComponent> ent,
        EntityUid uid,
        SCPStationGoalPrototype goalPrototype)
    {
        var scpName = MetaData(uid).EntityName;
        var message = Loc.GetString("scp-goals-announcement", ("name", scpName));
        var sender = Loc.GetString("scp-goals-announcement-sender");

        _chatSystem.DispatchGlobalAnnouncement(
            message,
            sender
        );

        SendSCPGoalToFax(goalPrototype, scpName);
    }

    private void SendSCPGoalToFax(SCPStationGoalPrototype goalPrototype, string scpName)
    {
        var goalMessage = Loc.GetString(goalPrototype.Message);
        var message = Loc.GetString("scp-goals-fax", ("name", scpName), ("task", goalMessage));

        var faxes = EntityQuery<FaxMachineComponent>();
        var printout = new FaxPrintout(
            message,
            "Задание отделу SCP",
            null,
            null,
            "paper_stamp-centcom",
            new List<StampDisplayInfo> { GetStampDisplayInfo() }
        );

        foreach (var fax in faxes)
        {
            if (!fax.ReceiveSCPGoal)
                continue;

            _faxSystem.Receive(fax.Owner, printout, null, fax);
        }
    }

    private StampDisplayInfo GetStampDisplayInfo()
    {
        var stampedName = Loc.GetString("stamp-component-stamped-name-centcom");
        var stampedColor = Color.TryFromHex("#006600");

        return new StampDisplayInfo
        {
            StampedName = stampedName,
            StampedColor = stampedColor ?? Color.DarkGreen,
        };
    }
}

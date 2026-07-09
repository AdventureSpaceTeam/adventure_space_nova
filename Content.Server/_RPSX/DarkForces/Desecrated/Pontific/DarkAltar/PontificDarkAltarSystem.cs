using System;
using System.Collections.Generic;
using System.Linq;
using Content.Server.RPSX.DarkForces.Saint.Items.Cross.Events;
using Content.Server.RPSX.CCvars;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Humanoid;
using Content.Shared.Radio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Content.Shared.Damage.Systems;

namespace Content.Server.RPSX.DarkForces.Desecrated.Pontific.DarkAltar;

public sealed class PontificDarkAltarSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private static readonly EntProtoId AltarPrototype = "PontificDarkAltar";
    private static readonly ProtoId<RadioChannelPrototype> RadioChannelPrototype = "Undead";
    private static readonly ProtoId<DamageTypePrototype> FelDamage = "Fel";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SaintedCrossFindingEvent>(OnSaintedCrossFinding);
    }

    private void OnSaintedCrossFinding(SaintedCrossFindingEvent ev)
    {
        if (ev.Handled)
            return;

        var coordinates = Transform(ev.Cross).Coordinates;
        var altars = _entityLookup.GetEntitiesInRange<PontificDarkAltarComponent>(coordinates, 7f);
        if (!altars.Any())
            return;

        ev.Handled = true;
        ev.Message = new SaintedCrossFindingEvent.SaintedCrossMessage(Loc.GetString("dark-altar-cross-finding"));
        ev.Colorize = new SaintedCrossFindingEvent.SaintedCrossColorize(Color.DarkRed, 3, 3);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<PontificDarkAltarComponent>();
        while (query.MoveNext(out var uid, out var darkAltarComponent))
        {
            if (darkAltarComponent.TickProduceFel <= _timing.CurTime)
            {
                ProduceFel((uid, darkAltarComponent));
                SendAltarMessage(uid, Loc.GetString("dark-altar-produce-fel"));
            }

            if (darkAltarComponent.TickIncreaseRadius <= _timing.CurTime)
            {
                IncreaseFelRadius((uid, darkAltarComponent));
                SendAltarMessage(uid, Loc.GetString("dark-altar-increase-fel-radius"));
            }
        }
    }

    private void ProduceFel(Entity<PontificDarkAltarComponent> altarEntity)
    {
        var component = altarEntity.Comp;
        var uid = altarEntity.Owner;

        var felProduceTimeConfig = TimeSpan.FromSeconds(_cfg.GetCVar(RPSXCVars.PontificFelProduceTime));
        var nextFelProduce = _timing.CurTime + felProduceTimeConfig;
        component.TickProduceFel = nextFelProduce;

        var radius = component.FelRadius;
        var coordinates = Transform(uid).Coordinates;

        var humans = _entityLookup.GetEntitiesInRange<HumanoidAppearanceComponent>(coordinates, radius);
        TryDamageHumansWithFel(humans);
    }

    private void TryDamageHumansWithFel(HashSet<Entity<HumanoidAppearanceComponent>> humans)
    {
        if (!humans.Any())
            return;

        var damage = _cfg.GetCVar(RPSXCVars.PontificFelRadiusDamage);
        var damageSpecifier = new DamageSpecifier();
        damageSpecifier.DamageDict.Add(FelDamage, damage);

        foreach (var human in humans)
        {
            _damageable.TryChangeDamage(human.Owner, damageSpecifier);
        }
    }

    private void IncreaseFelRadius(Entity<PontificDarkAltarComponent> altarEntity)
    {
        var increaseFelRadius = _cfg.GetCVar(RPSXCVars.PontificFelRadiusIncrease);
        var increaseRadiusTime = TimeSpan.FromSeconds(_cfg.GetCVar(RPSXCVars.PontificFelRadiusIncreaseTime));
        var tickIncreaseRadius = _timing.CurTime + increaseRadiusTime;

        altarEntity.Comp.TickIncreaseRadius = tickIncreaseRadius;
        altarEntity.Comp.FelRadius += increaseFelRadius;
    }

    private void SendAltarMessage(EntityUid altarUid, string message)
    {
        _radio.SendRadioMessage(altarUid, message, _prototype.Index<RadioChannelPrototype>(RadioChannelPrototype),
            altarUid);
    }

    public bool CanSpawnNewAltar(EntityUid uid)
    {
        if (!HasComp<PontificComponent>(uid))
            return false;

        return GetAltarCounts(uid) < 3;
    }

    public int GetAltarCounts(EntityUid uid)
    {
        if (!HasComp<PontificComponent>(uid))
            return 0;

        var altarsCount = 0;
        var query = EntityQueryEnumerator<PontificDarkAltarComponent>();
        while (query.MoveNext(out _, out var altarComponent))
        {
            if (altarComponent.AltarOwner == uid)
            {
                altarsCount++;
            }
        }

        return altarsCount;
    }

    public void SpawnNewAltar(EntityUid uid)
    {
        if (!HasComp<PontificComponent>(uid))
            return;

        var pontificCoors = Transform(uid).Coordinates;
        var altar = Spawn(AltarPrototype, pontificCoors);

        var altarComponent = EnsureComp<PontificDarkAltarComponent>(altar);
        altarComponent.AltarOwner = uid;

        IncreaseFelRadius((altar, altarComponent));
        ProduceFel((altar, altarComponent));
    }
}

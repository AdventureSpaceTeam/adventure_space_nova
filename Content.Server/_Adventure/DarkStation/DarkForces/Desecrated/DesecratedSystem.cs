using Content.Server._Adventure.DarkForces.Saint.Items.Cross.Events;
using Content.Server._Adventure.DarkForces.Saint.Reagent.Events;
using Content.Server._Adventure.DarkForces.Saint.Saintable.Events;
using Content.Server.Polymorph.Systems;
using Content.Server.Popups;
using Content.Shared._Adventure.DarkForces.Desecrated;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Polymorph;
using Robust.Shared.Physics.Events;

namespace Content.Server._Adventure.DarkForces.Desecrated;

public sealed class DesecratedSystem : EntitySystem
{
    [ValidatePrototypeId<PolymorphPrototype>]
    private const string DesecratedPolymorph = "DesecrateMobDesecratedPolymorph";

    [ValidatePrototypeId<DamageTypePrototype>]
    private const string FelDamage = "Fel";

    [ValidatePrototypeId<DamageTypePrototype>]
    private const string ShockDamage = "Shock";

    [ValidatePrototypeId<DamageTypePrototype>]
    private const string BurnDamage = "Burn";

    [ValidatePrototypeId<DamageTypePrototype>]
    private const string HolinessDamage = "Holiness";

    private static readonly DamageSpecifier SaintWaterDamage = new()
    {
        DamageDict =
        {
            { "HolinessDamage", 10 },
        },
    };

    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly PolymorphSystem _polymorph = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged, after: new[] { typeof(PolymorphSystem) });
        SubscribeLocalEvent<SaintedCrossFindingEvent>(OnSaintedCrossFinding);
        SubscribeLocalEvent<DesecratedLightningComponent, StartCollideEvent>(OnDesecratedLightningCollide);

        SubscribeLocalEvent<DesecratedMarkerComponent, OnSaintEntityAfterInteract>(OnSaintItemsContact);
        SubscribeLocalEvent<DesecratedMarkerComponent, OnSaintEntityCollide>(OnSaintItemsContact);
        SubscribeLocalEvent<DesecratedMarkerComponent, OnSaintWaterFlammableEvent>(OnSaintWaterFlammable);
        SubscribeLocalEvent<DesecratedMarkerComponent, OnTryPryingSaintedEvent>(OnSaintItemsContact);
        SubscribeLocalEvent<DesecratedMarkerComponent, PolymorphRevertedEvent>(OnPolymorphRevertedEvent);
    }

    private void OnPolymorphRevertedEvent(EntityUid uid,
        DesecratedMarkerComponent component,
        PolymorphRevertedEvent args)
    {
        var target = args.Original;

        if (!TryComp<DamageableComponent>(target, out var damageable) ||
            !TryComp<DamageableComponent>(uid, out var desecreateDamage))
            return;

        var newDamage = new DamageSpecifier
        {
            DamageDict = new Dictionary<string, FixedPoint2>(),
        };

        foreach (var damage in desecreateDamage.Damage.DamageDict)
        {
            newDamage.DamageDict[damage.Key] = damage.Value;
        }

        desecreateDamage.Damage.DamageDict.TryGetValue(HolinessDamage, out var holinessDamage);

        newDamage.DamageDict[HolinessDamage] = 0;
        newDamage.DamageDict[FelDamage] = holinessDamage;

        _damageable.SetDamage(target, damageable, newDamage);
    }

    private void OnSaintWaterFlammable(EntityUid uid,
        DesecratedMarkerComponent component,
        OnSaintWaterFlammableEvent args)
    {
        if (args.Cancelled)
            return;

        _damageable.TryChangeDamage(uid, SaintWaterDamage);
        args.Cancel();
    }

    private void OnSaintItemsContact(EntityUid uid, DesecratedMarkerComponent component, ISaintEntityEvent args)
    {
        args.PushOnCollide = true;
        args.IsHandled = true;
        args.DamageOnCollide *= 1.2;

        _popup.PopupEntity(Loc.GetString("pontific-mob-saint-contact"), uid, uid);
    }

    private void OnSaintedCrossFinding(SaintedCrossFindingEvent ev)
    {
        if (ev.Handled)
            return;

        var crossTransform = Transform(ev.Cross);
        var humans = _entityLookup.GetEntitiesInRange<HumanoidAppearanceComponent>(crossTransform.Coordinates, 6f);
        foreach (var humanUid in humans)
        {
            if (!TryComp<DamageableComponent>(humanUid, out var damageableComponent))
                continue;

            var felDamage = damageableComponent.Damage.DamageDict[FelDamage];
            if (felDamage <= 0)
                continue;

            OnSaintCrossHandledFel(ev);
            break;
        }
    }

    private void OnSaintCrossHandledFel(SaintedCrossFindingEvent args)
    {
        args.Handled = true;
        args.Message = new SaintedCrossFindingEvent.SaintedCrossMessage(Loc.GetString("pontific-saint-cross"));
        args.Colorize = new SaintedCrossFindingEvent.SaintedCrossColorize(Color.Aqua, 3, 3);
    }

    private void OnDesecratedLightningCollide(EntityUid uid,
        DesecratedLightningComponent component,
        ref StartCollideEvent args)
    {
        var target = args.OtherEntity;
        if (component.DoubleAttackConvert && HasComp<DesecratedTargetComponent>(target))
        {
            ConvertToDesecrated(args.OtherEntity);
            return;
        }

        ApplyDesecratedLightningDamage(target, component);
        HandleTwiceAttackMark(target, component);
    }

    private void ApplyDesecratedLightningDamage(EntityUid target, DesecratedLightningComponent component)
    {
        var damageSpecifier = new DamageSpecifier();

        damageSpecifier.DamageDict.Add(FelDamage, component.FelDamage);
        damageSpecifier.DamageDict.Add(ShockDamage, component.FelDamage * 0.5);
        damageSpecifier.DamageDict.Add(BurnDamage, component.FelDamage * 0.2);

        _damageable.TryChangeDamage(target, damageSpecifier);
    }

    private void HandleTwiceAttackMark(EntityUid target, DesecratedLightningComponent component)
    {
        if (!component.DoubleAttackConvert || !TargetCanConverted(target))
            return;

        EnsureComp<DesecratedTargetComponent>(target);
    }

    private void OnMobStateChanged(MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead)
            return;

        var target = args.Target;
        if (!TryComp<DamageableComponent>(target, out var damageableComponent))
            return;

        if (!damageableComponent.Damage.DamageDict.TryGetValue(FelDamage, out var felDamage))
            return;

        if (damageableComponent.TotalDamage / 2 > felDamage || felDamage == 0)
            return;

        ConvertToDesecrated(target);
    }

    private void ConvertToDesecrated(EntityUid target)
    {
        if (!TargetCanConverted(target))
            return;

        if (TryComp<DamageableComponent>(target, out var damageablecomp))
            _damageable.SetAllDamage(target, damageablecomp, 0);

        _mobStateSystem.ChangeMobState(target, MobState.Alive);
        _polymorph.PolymorphEntity(target, DesecratedPolymorph);
    }

    private bool TargetCanConverted(EntityUid target)
    {
        return HasComp<HumanoidAppearanceComponent>(target) && !HasComp<DesecratedMarkerComponent>(target);
    }
}

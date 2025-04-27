using Content.Server._Adventure.DarkForces.Desecrated.Pontific.Bonus;
using Content.Server._Adventure.DarkForces.Desecrated.Pontific.DarkAltar;
using Content.Server._Adventure.DarkForces.Desecrated.Pontific.Prayer;
using Content.Server._Adventure.SecretCCVars;
using Content.Server.Beam;
using Content.Shared._Adventure.DarkForces.Desecrated;
using Content.Shared._Adventure.Hunter.Desecrated.Pontific;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Server.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using PontificBloodyAltarEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificBloodyAltarEvent;
using PontificDarkPrayerEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificDarkPrayerEvent;
using PontificFelLightningEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificFelLightningEvent;
using PontificFlameSwordsEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificFlameSwordsEvent;
using PontificKudzuEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificKudzuEvent;
using PontificLungeOfFaithEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificLungeOfFaithEvent;
using PontificPrayerDoAfterEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificPrayerDoAfterEvent;
using PontificSpawnGuardianEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificSpawnGuardianEvent;
using PontificSpawnMonkEvent = Content.Shared._Adventure.Hunter.Desecrated.Pontific.PontificSpawnMonkEvent;

namespace Content.Server._Adventure.DarkForces.Desecrated.Pontific;

public sealed partial class PontificSystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string MobFallenGuard = "MobFallenGuard";

    [ValidatePrototypeId<EntityPrototype>]
    private const string MobCursedMonk = "MobCursedMonk";

    [ValidatePrototypeId<EntityPrototype>]
    private const string PontificKudzu = "PontificDarkAltarKudzu";

    private const int BasicFelCount = 30;

    private static readonly TimeSpan PrayerDelay = TimeSpan.FromSeconds(45);
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly BeamSystem _beam = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly PontificBonusSystem _pontificBonus = default!;
    [Dependency] private readonly PontificDarkAltarSystem _pontificDarkAltar = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    private void InitAbilities()
    {
        SubscribeLocalEvent<PontificComponent, PontificSpawnGuardianEvent>(OnSpawnGuardian);
        SubscribeLocalEvent<PontificComponent, PontificSpawnMonkEvent>(OnSpawnMonk);
        SubscribeLocalEvent<PontificComponent, PontificBloodyAltarEvent>(OnSpawnAltar);
        SubscribeLocalEvent<PontificComponent, PontificDarkPrayerEvent>(OnDarkPrayer);
        SubscribeLocalEvent<PontificComponent, PontificFelLightningEvent>(OnFelLighting);
        SubscribeLocalEvent<PontificComponent, PontificFlameSwordsEvent>(OnFlameSwords);
        SubscribeLocalEvent<PontificComponent, PontificLungeOfFaithEvent>(OnLungeOfFaith);
        SubscribeLocalEvent<PontificComponent, PontificKudzuEvent>(OnPontificKudzu);
        SubscribeLocalEvent<PontificComponent, MobStateChangedEvent>(OnMobChangedState);

        SubscribeLocalEvent<PontificComponent, PontificBonusEndEvent>(OnPontificBonusEnd);
        SubscribeLocalEvent<PontificComponent, PontificPrayerDoAfterEvent>(OnPontificPrayerEnd);
    }

    private void OnPontificKudzu(EntityUid uid, PontificComponent component, PontificKudzuEvent args)
    {
        if (args.Handled || !CanUseAbility(uid, component, args))
            return;

        Spawn(PontificKudzu, Transform(uid).Coordinates);
        UpdateFelCount(uid, component, args);

        args.Handled = true;
    }

    private void OnPontificPrayerEnd(EntityUid uid, PontificComponent component, PontificPrayerDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || _mobStateSystem.IsDead(uid))
            return;

        var felCount = BasicFelCount;
        var altarCounts = _pontificDarkAltar.GetAltarCounts(uid);
        if (altarCounts != 0)
        {
            felCount = (int)(felCount * (altarCounts * 1.75));
            HealNearbly((uid, component));
        }

        UpdateFelCount(uid, component, -felCount);

        args.Handled = true;
    }

    private void HealNearbly(Entity<PontificComponent> pontific)
    {
        var transform = Transform(pontific.Owner);
        var desecrates = _entityLookup.GetEntitiesInRange<DesecratedMarkerComponent>(transform.Coordinates, 4f);

        var healingDamage = pontific.Comp.HealingDamage;
        foreach (var desecrate in desecrates)
        {
            if (_mobStateSystem.IsDead(desecrate))
                continue;

            var isPontific = pontific.Owner == desecrate.Owner;
            _damageableSystem.TryChangeDamage(desecrate, isPontific ? healingDamage * 1.5 : healingDamage);
        }
    }

    private void OnPontificBonusEnd(EntityUid uid, PontificComponent component, PontificBonusEndEvent args)
    {
        if (args.Key != "flame" || _mobStateSystem.IsDead(uid))
            return;

        _appearance.SetData(uid, PontificStateVisuals.State, PontificState.Base);
    }

    private void OnMobChangedState(EntityUid uid, PontificComponent component, MobStateChangedEvent args)
    {
        switch (args.NewMobState)
        {
            case MobState.Alive:
                _appearance.SetData(uid, PontificStateVisuals.State, PontificState.Base);
                break;
            case MobState.Dead:
                _appearance.SetData(uid, PontificStateVisuals.State, PontificState.Dead);
                break;
            default:
                _appearance.SetData(uid, PontificStateVisuals.State, PontificState.Base);
                break;
        }
    }

    private void OnLungeOfFaith(EntityUid uid, PontificComponent component, PontificLungeOfFaithEvent args)
    {
        if (args.Handled || !CanUseAbility(uid, component, args))
            return;

        var faithTime = _cfg.GetCVar(PontificCVars.PontificFaithTime);

        _pontificBonus.StartFaith(uid, "faith", faithTime, 1.25f, 1.25f);
        _popup.PopupEntity(Loc.GetString("pontific-lunge-of-faith"), uid, uid);

        UpdateFelCount(uid, component, args);

        args.Handled = true;
    }

    private void OnFlameSwords(EntityUid uid, PontificComponent component, PontificFlameSwordsEvent args)
    {
        if (args.Handled || !CanUseAbility(uid, component, args))
            return;

        if (HasComp<PontificPrayerComponent>(uid))
        {
            _popup.PopupEntity(Loc.GetString("pontific-flame-swords-prayer"), uid, uid);
            return;
        }

        var flameTime = _cfg.GetCVar(PontificCVars.PontificFlameTime);

        _pontificBonus.StartFaith(uid, "flame", flameTime, 1.25f, 1.75f);
        _appearance.SetData(uid, PontificStateVisuals.State, PontificState.Flame);
        UpdateFelCount(uid, component, args);

        args.Handled = true;
    }

    private void OnFelLighting(EntityUid uid, PontificComponent component, PontificFelLightningEvent args)
    {
        if (args.Handled || !CanUseAbility(uid, component, args))
            return;

        _beam.TryCreateBeam(uid, args.Target, "PontificLightning");
        UpdateFelCount(uid, component, args);

        args.Handled = true;
    }

    private void OnDarkPrayer(EntityUid uid, PontificComponent component, PontificDarkPrayerEvent args)
    {
        if (args.Handled || !CanUseAbility(uid, component, args))
            return;

        if (TryComp<PontificBonusComponent>(uid, out var bonusComponent) && bonusComponent.Key == "flame")
        {
            _popup.PopupEntity(Loc.GetString("pontific-prayer-blocked"), uid, uid);
            return;
        }

        var ev = new PontificPrayerDoAfterEvent();
        var doAfterArgs = new DoAfterArgs(
            EntityManager,
            uid,
            PrayerDelay,
            ev,
            uid,
            null,
            uid
        )
        {
            BreakOnMove = true,
            MovementThreshold = 4f,
        };
        _doAfter.TryStartDoAfter(doAfterArgs);
        _audioSystem.PlayEntity(component.PrayerSound, Filter.Pvs(uid, entityManager: EntityManager), uid, true);

        args.Handled = true;
    }

    private void OnSpawnAltar(EntityUid uid, PontificComponent component, PontificBloodyAltarEvent args)
    {
        if (args.Handled || !CanUseAbility(uid, component, args))
            return;

        if (!_pontificDarkAltar.CanSpawnNewAltar(uid))
        {
            _popup.PopupEntity(Loc.GetString("pontific-dark-altar-spawn-blocked"), uid, uid);
            return;
        }

        _pontificDarkAltar.SpawnNewAltar(uid);
        UpdateFelCount(uid, component, args);

        args.Handled = true;
    }

    private void OnSpawnMonk(EntityUid uid, PontificComponent component, PontificSpawnMonkEvent args)
    {
        if (args.Handled || !CanUseAbility(uid, component, args))
            return;

        SpawnMob((uid, component), args, MobCursedMonk);
        args.Handled = true;
    }

    private void OnSpawnGuardian(EntityUid uid, PontificComponent component, PontificSpawnGuardianEvent args)
    {
        if (args.Handled || !CanUseAbility(uid, component, args))
            return;

        SpawnMob((uid, component), args, MobFallenGuard);
        args.Handled = true;
    }

    private void SpawnMob(Entity<PontificComponent> pontific, IPontificAction args, string prototypeId)
    {
        var pontificCoords = Transform(pontific).Coordinates;
        Spawn(prototypeId, pontificCoords);
        UpdateFelCount(pontific, pontific, args);
    }

    private bool CanUseAbility(EntityUid uid, PontificComponent component, IPontificAction action)
    {
        if (component.PontificFel >= action.FelCost)
            return true;

        _popup.PopupEntity(Loc.GetString("pontific-not-enough-fel"), uid, uid);
        return false;
    }
}

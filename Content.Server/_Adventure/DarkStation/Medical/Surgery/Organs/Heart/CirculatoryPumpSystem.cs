using System.Linq;
using Content.Server.Body.Components;
using Content.Server.Body.Events;
using Content.Shared.AdventureSpace.Medical.Surgery.Events.Organs;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using JetBrains.Annotations;
using Robust.Shared.Configuration;
using Robust.Shared.Random;

namespace Content.Server.AdventureSpace.Medical.Surgery.Organs.Heart;

[UsedImplicitly]
public sealed class CirculatoryPumpSystem : SurgeryOrganSystem<CirculatoryPumpComponent, CirculatoryPumpMarkerComponent>
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SurgerySystem _surgerySystem = default!;

    private bool _isHeartAttackEnabled;
    private bool _isHeartStrainEnabled;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CirculatoryPumpComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<CirculatoryPumpComponent, SurgeryRequestPump>(OnRequestPump);
        SubscribeLocalEvent<CirculatoryPumpComponent, OnEntitySaturationAttempt>(OnSaturationAttempt);
        SubscribeLocalEvent<CanProcessEntitySaturation>(OnProcessSaturation);

        Subs.CVar(_cfg, SecretCCVars.SecretCCVars.IsHeartAttackEnabled, value => _isHeartAttackEnabled = value);
        Subs.CVar(_cfg, SecretCCVars.SecretCCVars.IsHeartStrainEnabled, value => _isHeartStrainEnabled = value);
    }

    private void OnProcessSaturation(ref CanProcessEntitySaturation ev)
    {
        ev.IgnoreAttempt = false;
    }

    private void OnSaturationAttempt(Entity<CirculatoryPumpComponent> ent, ref OnEntitySaturationAttempt args)
    {
        args.HasSaturation = ent.Comp.Working;
    }

    private void OnRequestPump(Entity<CirculatoryPumpComponent> ent, ref SurgeryRequestPump args)
    {
        StartPump(ent);
    }

    private void OnComponentInit(EntityUid uid, CirculatoryPumpComponent component, ComponentInit args)
    {
        component.WarningThreshold = component.MaxDamageThreshold * 0.5f;
        component.CriticalThreshold = component.MaxDamageThreshold * 0.75f;
    }

    protected override void OnOrganInterval(Entity<CirculatoryPumpComponent> ent)
    {
        //if the mob is dead, stop the heart and move one
        if (_mobState.IsDead(ent) && ent.Comp.Working)
        {
            SetFailure(ent);
            UpdateOrganStatus(ent);

            return;
        }

        if (!ent.Comp.Working && !_mobState.IsDead(ent))
        {
            _damageable.TryChangeDamage(ent, ent.Comp.NotWorkingDamage, true, origin: ent);
            return;
        }

        CheckBrainless(ent);

        if (!TryComp<HungerComponent>(ent, out var hunger))
            return;

        CheckOverfed(ent, (ent, hunger));
        CheckHeartAttack(ent);

        UpdateOrganStatus(ent);
    }

    private void CheckBrainless(Entity<CirculatoryPumpComponent> ent)
    {
        if (ent.Comp.Brainless)
            return;

        var organs = _surgerySystem.GetAllBodyOrgans(ent);
        var hasBrain = organs.Any(HasComp<BrainComponent>);
        if (!hasBrain)
            SetFailure(ent);
    }

    private void CheckOverfed(Entity<CirculatoryPumpComponent> ent, Entity<HungerComponent> entHunger)
    {
        var pump = ent.Comp;
        var hunger = entHunger.Comp;
        if (hunger.OverfedStrain > 0)
        {
            var mod = entHunger.Comp.OverfedStrain - ent.Comp.StrainMod;
            if (ent.Comp.Strain < mod && ent.Comp.StrainCeiling < mod)
            {
                pump.Strain = hunger.OverfedStrain - pump.StrainMod;
                pump.StrainCeiling = pump.Strain;
            }
        }
        else
            pump.StrainCeiling = 0;

        pump.Strain = Math.Max(0, pump.Strain - pump.StrainRecovery);

        if (pump.Strain <= 0)
            return;

        if (!_isHeartStrainEnabled)
            return;

        var roll = _random.Next(1, 100);
        if (roll > pump.Strain * pump.StrainDamageMod * 100)
            return;

        var newDamage = pump.Damage + pump.Strain * pump.StrainDamageMod;
        pump.Damage += Math.Min(newDamage, pump.MaxDamageThreshold);
    }

    private void CheckHeartAttack(Entity<CirculatoryPumpComponent> ent)
    {
        if (!_isHeartAttackEnabled)
            return;
        //take the current damage and roll to incur a heart attack
        if (!(ent.Comp.Damage >= ent.Comp.MinDamageThreshold))
            return;

        var roll = _random.Next(1, 100);
        if (roll > (ent.Comp.Damage - ent.Comp.MinDamageThreshold) * ent.Comp.DamageMod * 100)
            return;

        SetFailure(ent);
    }

    private void StartPump(Entity<CirculatoryPumpComponent> ent)
    {
        if (_mobState.IsDead(ent))
            return;

        ent.Comp.Working = true;
        UpdateOrganStatus(ent);
    }

    protected override void OnRejuvenate(Entity<CirculatoryPumpComponent> ent)
    {
        ent.Comp.Strain = 0f;
        ent.Comp.StrainCeiling = 0f;
    }

    protected override void OnAddedToBody(Entity<CirculatoryPumpComponent> ent, Entity<CirculatoryPumpComponent> oldEnt)
    {
        StartPump(ent);
    }
}

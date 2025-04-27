using System.Linq;
using Content.Server.AdventureSpace.Medical.Surgery.Body;
using Content.Server.AdventureSpace.Medical.Surgery.Components;
using Content.Server.AdventureSpace.Medical.Surgery.Events;
using Content.Server.Body.Systems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Kitchen.Components;
using Content.Shared.AdventureSpace.Medical.Surgery;
using Content.Shared.AdventureSpace.Medical.Surgery.Components;
using Content.Shared.Armor;
using Content.Shared.Body.Part;
using Content.Shared.Damage;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Projectiles;
using Robust.Shared.Configuration;
using Robust.Shared.Random;

namespace Content.Server.AdventureSpace.Medical.Surgery.Wounds;

public sealed partial class SurgeryDamageSystem : EntitySystem
{
    [Dependency] private readonly BodySystem _bodySystem = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly SurgeryBodySystem _surgeryBodySystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private bool _isSeveredLimbEnabled;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SurgeryComponent, DamageChangedEvent>(OnMeleeHit);
        SubscribeLocalEvent<SurgeryComponent, ExplosionHitEvent>(OnExplosionHit);

        SubscribeLocalEvent<SurgeryWoundablePartComponent, SurgeryAttachPartAttemptEvent>(OnAttachAttempt);

        InitHealing();

        _isSeveredLimbEnabled = _cfg.GetCVar(SecretCCVars.SecretCCVars.IsSeveredLimbEnabled);

        _cfg.OnValueChanged(SecretCCVars.SecretCCVars.IsSeveredLimbEnabled,
            value => _isSeveredLimbEnabled = value,
            true);
    }

    private void OnAttachAttempt(Entity<SurgeryWoundablePartComponent> ent, ref SurgeryAttachPartAttemptEvent args)
    {
        if (!(ent.Comp.Damage >= ent.Comp.MaxDamage))
            return;

        args.Canceled = true;
        args.Reason = "Эта часть тела уничтожена";
    }

    private void OnExplosionHit(Entity<SurgeryComponent> ent, ref ExplosionHitEvent args)
    {
        if (!IsSeveredLimbAvailable(ent))
            return;

        var bodyPart = GetRandomBodyPart(ent);
        if (bodyPart is not { } targetBodyPart)
            return;

        var basicDamage = args.Damage.GetTotal().Float();
        var damage = GetArmor(ent, basicDamage, targetBodyPart);

        UpdateDamage(ent, targetBodyPart, damage);
    }

    private void OnMeleeHit(Entity<SurgeryComponent> ent, ref DamageChangedEvent args)
    {
        if (!IsSeveredLimbAvailable(ent))
            return;

        if (args.Origin is not { } damager)
            return;

        if (!TryComp<TargetDollComponent>(damager, out var damagerDoll))
            return;

        if (args.DamageDelta is not { } delta)
            return;

        if (!IsWeaponValid(args.Weapon))
            return;

        var bodyPart = GetTargetDollBodyPart(ent, (damager, damagerDoll));
        if (bodyPart is not { } targetBodyPart)
            return;

        if (!targetBodyPart.Owner.IsValid() || !targetBodyPart.Comp2.CanBeDollTarget)
            return;

        var basicDamage = delta.GetTotal().Float();
        var damage = GetArmor(ent, basicDamage, targetBodyPart);

        UpdateDamage(ent, targetBodyPart, damage);
    }

    private bool IsWeaponValid(EntityUid? weapon)
    {
        if (HasComp<SharpComponent>(weapon))
            return true;

        return TryComp<ProjectileComponent>(weapon, out var projectile) && projectile.DamageBodyParts;
    }

    private bool IsSeveredLimbAvailable(EntityUid uid)
    {
        return _isSeveredLimbEnabled && HasComp<HumanoidAppearanceComponent>(uid);
    }

    private float GetArmor(EntityUid uid, float damage, Entity<BodyPartComponent> bodyPart)
    {
        if (!TryComp(uid, out InventoryComponent? inv))
            return damage;

        var partType = bodyPart.Comp.PartType;
        var slotEnumerator = new InventorySystem.InventorySlotEnumerator(inv);
        while (slotEnumerator.MoveNext(out var slot))
        {
            if (slot.ContainedEntity is not { } ent)
                continue;

            if (!TryComp<ArmorComponent>(ent, out var armor) ||
                !armor.SecurePartTypes.TryGetValue(partType, out var bodyPartCoefficients))
                continue;

            damage *= bodyPartCoefficients;
        }

        return damage;
    }

    private Entity<BodyPartComponent, SurgeryWoundablePartComponent>? GetRandomBodyPart(EntityUid uid)
    {
        var bodyParts = _surgeryBodySystem.GetBodyChildren<SurgeryWoundablePartComponent>(uid)
            .Where(part => part.Component.CanBeExplosionTarget)
            .ToList();

        if (bodyParts.Any())
            return null;

        return _robustRandom.Pick(bodyParts);
    }

    private Entity<BodyPartComponent, SurgeryWoundablePartComponent>? GetTargetDollBodyPart(EntityUid uid,
        Entity<TargetDollComponent> damager)
    {
        var symmetry = damager.Comp.BodyPartSymmetry;
        var targetPart = damager.Comp.TargetBodyPart;
        if (targetPart == null)
            return null;

        var bodyPart = _surgeryBodySystem
            .GetBodyChildrenOfType<SurgeryWoundablePartComponent>(uid, targetPart.Value, symmetry)
            .FirstOrDefault();

        return bodyPart;
    }

    private void UpdateDamage(EntityUid uid, Entity<BodyPartComponent, SurgeryWoundablePartComponent> ent, float damage)
    {
        ent.Comp2.Damage += damage;

        if (ent.Comp2.Damage < ent.Comp2.MaxDamage)
            return;

        ent.Comp2.Damage = ent.Comp2.MaxDamage;
        _bodySystem.RequestRemovePart(ent);

        var userTransform = Transform(uid);
        _transform.SetCoordinates(ent, userTransform.Coordinates);
    }
}

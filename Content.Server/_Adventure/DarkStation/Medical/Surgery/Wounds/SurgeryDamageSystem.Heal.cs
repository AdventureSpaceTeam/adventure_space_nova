using System.Linq;
using Content.Server.AdventureSpace.Medical.Surgery.Components;
using Content.Server.Medical;
using Content.Shared.AdventureSpace.Medical.Surgery.Components;
using Content.Shared.Damage;
using Content.Shared.Rejuvenate;

namespace Content.Server.AdventureSpace.Medical.Surgery.Wounds;

public sealed partial class SurgeryDamageSystem
{
    private void InitHealing()
    {
        SubscribeLocalEvent<SurgeryComponent, RejuvenateEvent>(OnRejuvenate);
        SubscribeLocalEvent<SurgeryComponent, EntityHealedEvent>(OnEntityHealed);
    }

    private void OnRejuvenate(Entity<SurgeryComponent> ent, ref RejuvenateEvent args)
    {
        HealAllBodyParts(ent);
    }

    private void OnEntityHealed(Entity<SurgeryComponent> ent, ref EntityHealedEvent args)
    {
        if (TryComp(ent, out DamageableComponent? damageable) && damageable.TotalDamage.Float() == 0)
        {
            HealAllBodyParts(ent);
            return;
        }

        var parts = _surgeryBodySystem
            .GetBodyChildren<SurgeryWoundablePartComponent>(ent)
            .Where(part => part.Component.Damage > 0)
            .ToList();

        var healPerPart = args.Healed / parts.Count();
        foreach (var (_, _, surgeryComp) in parts)
        {
            surgeryComp.Damage += healPerPart.Float();
            surgeryComp.Damage = surgeryComp.Damage <= 0 ? 0 : surgeryComp.Damage;
        }
    }

    private void HealAllBodyParts(Entity<SurgeryComponent> ent)
    {
        var parts = _bodySystem.GetBodyChildren(ent);
        foreach (var (partUid, _) in parts)
        {
            if (!TryComp<SurgeryWoundablePartComponent>(partUid, out var surgeryComp))
                continue;

            surgeryComp.Damage = 0;
        }
    }
}

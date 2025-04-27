using Content.Server.AdventureSpace.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;
using Content.Server.AdventureSpace.DarkForces.Narsi.Cultist.Roles.Narsi;
using Content.Shared.AdventureSpace.DarkForces.Narsi.Roles;
using Content.Shared.Damage;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Cultist.Roles.Cultists;

public sealed class NarsiNewCultistSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<NarsiCultistComponent, BeforeDamageChangedEvent>(BeforeDamageChanged);
        SubscribeLocalEvent<NarsiComponent, BeforeDamageChangedEvent>(OnBeforeDamageChangedNarsi);
    }

    private void OnBeforeDamageChangedNarsi(EntityUid uid, NarsiComponent component, ref BeforeDamageChangedEvent args)
    {
        args.Cancelled = true;
    }

    private void BeforeDamageChanged(EntityUid uid, NarsiCultistComponent component, ref BeforeDamageChangedEvent args)
    {
        if (args.Origin == null ||
            !HasComp<NarsiComponent>(args.Origin) && !HasComp<NarsiPolymorphComponent>(args.Origin))
            return;

        args.Cancelled = true;
    }
}

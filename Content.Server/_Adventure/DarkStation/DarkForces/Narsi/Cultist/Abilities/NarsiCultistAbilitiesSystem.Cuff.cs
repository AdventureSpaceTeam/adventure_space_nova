using Content.Server.Cuffs;
using Content.Shared.AdventureSpace.DarkForces.Narsi.Roles;
using Content.Shared.Stunnable;
using NarsiCultistCuffEvent = Content.Shared.AdventureSpace.DarkForces.Narsi.Abilities.Events.NarsiCultistCuffEvent;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Cultist.Abilities;

public sealed partial class NarsiCultistAbilitiesSystem
{
    [Dependency] private readonly CuffableSystem _cuffableSystem = default!;

    private void InitializeCuff()
    {
        SubscribeLocalEvent<NarsiCultistComponent, NarsiCultistCuffEvent>(OnCuffEvent);
    }

    private void OnCuffEvent(EntityUid uid, NarsiCultistComponent component, NarsiCultistCuffEvent args)
    {
        if (args.Handled)
            return;

        if (!HasComp<StunnedComponent>(args.Target))
            return;

        var coords = Transform(args.Performer).Coordinates;
        var handcuffs = Spawn("HandcuffsCult", coords);

        _cuffableSystem.TryCuffing(args.Performer, args.Target, handcuffs);
        OnCultistAbility(uid, args);
        args.Handled = true;
    }
}

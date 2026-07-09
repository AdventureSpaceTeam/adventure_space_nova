using Content.Server.Cuffs;
using Content.Shared.RPSX.DarkForces.Narsi.Abilities.Events;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Server.RPSX.DarkForces.Narsi.Cultist.Abilities;

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

        var coords = Transform(args.Performer).Coordinates;
        var handcuffs = Spawn("HandcuffsCult", coords);
        _handsSystem.TryPickup(uid, handcuffs);
        var cuffing = _cuffableSystem.TryCuffing(args.Performer, args.Target, handcuffs);
        OnCultistAbility(uid, args);
        args.Handled = true;
    }
}

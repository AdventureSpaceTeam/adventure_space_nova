using System;
using Content.Shared.RPSX.DarkForces.Narsi.Abilities.Events;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.DarkForces.Narsi.Cultist.Abilities;

public sealed partial class NarsiCultistAbilitiesSystem
{
    [Dependency] private readonly SharedStunSystem _stunSystem = default!;

    private void InitializeStun()
    {
        SubscribeLocalEvent<NarsiCultistComponent, NarsiCultistStunEvent>(OnStunEvent);
    }

    private void OnStunEvent(EntityUid uid, NarsiCultistComponent component, NarsiCultistStunEvent args)
    {
        if (args.Handled)
            return;

        var target = args.Target;
        var level = _progressSystem.GetAbilityLevel(StunAction);
        var duration = level switch
        {
            1 => 5,
            2 => 10,
            _ => 15
        };

        _stunSystem.TryAddParalyzeDuration(target, TimeSpan.FromSeconds(duration));
        OnCultistAbility(uid, args);

        args.Handled = true;
    }
}

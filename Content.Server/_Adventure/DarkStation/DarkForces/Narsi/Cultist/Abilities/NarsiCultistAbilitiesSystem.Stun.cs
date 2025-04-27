using Content.Shared.AdventureSpace.DarkForces.Narsi.Roles;
using Content.Shared.Stunnable;
using NarsiCultistStunEvent = Content.Shared.AdventureSpace.DarkForces.Narsi.Abilities.Events.NarsiCultistStunEvent;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Cultist.Abilities;

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
            _ => 15,
        };

        _stunSystem.TryParalyze(target, TimeSpan.FromSeconds(duration), true);
        OnCultistAbility(uid, args);

        args.Handled = true;
    }
}

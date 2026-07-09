using Content.Server.Beam;
using Content.Shared.Actions;
using Content.Shared.RPSX.DarkForces.Desecrated;

namespace Content.Server.RPSX.DarkForces.Desecrated.CursedMonk;

public sealed class CursedMonkSystem : EntitySystem
{
    [Dependency] private readonly BeamSystem _beam = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CursedMonkComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<CursedMonkComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<CursedMonkComponent, AttackLightningEvent>(OnAttackLightning);
    }

    private void OnStartup(EntityUid uid, CursedMonkComponent component, ComponentStartup args)
    {
        _actionsSystem.AddAction(uid, ref component.LightningActionEntity, component.LightingAction);
    }

    private void OnShutdown(EntityUid uid, CursedMonkComponent component, ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(uid, component.LightningActionEntity);
    }

    private void OnAttackLightning(EntityUid uid, CursedMonkComponent component, AttackLightningEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        _beam.TryCreateBeam(uid, args.Target, component.ZapBeamEntityId);
    }
}

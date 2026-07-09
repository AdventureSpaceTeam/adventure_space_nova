using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.RPSX.DarkForces.Desecrated.Pontific;

namespace Content.Server.RPSX.DarkForces.Desecrated.Pontific;

public sealed partial class PontificSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _sharedActions = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PontificComponent, ComponentInit>(OnPontificInit);
        SubscribeLocalEvent<PontificComponent, ComponentShutdown>(OnPontificShutdown);

        InitAbilities();
    }

    private void OnPontificInit(EntityUid uid, PontificComponent component, ComponentInit args)
    {
        _appearance.SetData(uid, PontificStateVisuals.State, PontificState.Base);
        foreach (var action in component.PontificActionsList)
        {
            EntityUid? entityUid = null;
            _sharedActions.AddAction(uid, ref entityUid, action, uid);
            component.PontificActions[action] = entityUid;
        }

        UpdateFelCount(uid, component, 0);
    }

    private void UpdateFelCount(EntityUid uid, PontificComponent component, IPontificAction action)
    {
        UpdateFelCount(uid, component, action.FelCost);
    }

    private void UpdateFelCount(EntityUid uid, PontificComponent component, int felCount)
    {
        component.PontificFel -= felCount;
        component.PontificFel = component.PontificFel switch
        {
            < 0 => 0,
            > 300 => 300,
            _ => component.PontificFel
        };

        var alertFelCount = component.PontificFel / 300f * 10;
        var severity = (short)Math.Clamp(alertFelCount, 0, 10);
        _alerts.ShowAlert(uid, component.PontificFelAlert, severity);
    }

    private void OnPontificShutdown(EntityUid uid, PontificComponent component, ComponentShutdown args)
    {
        var actions = _sharedActions.GetActions(uid);
        foreach (var action in actions)
        {
            if (_sharedActions.GetEvent(action) is not IPontificAction)
                continue;

            _sharedActions.RemoveAction(uid, action.Owner);
        }
    }
}

using Content.Server.Popups;
using Content.Shared.RPSX.DarkForces.Desecrated.Pontific;

namespace Content.Server.RPSX.DarkForces.Desecrated.Pontific;

public sealed partial class PontificFel : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PontificFelAlertEvent>(OnFelAlert);
    }

    public void OnFelAlert(PontificFelAlertEvent args)
    {
        var entManager = IoCManager.Resolve<IEntityManager>();
        if (!entManager.TryGetComponent<PontificComponent>(args.User, out var pontificComponent))
            return;

        var popup = entManager.System<PopupSystem>();
        var message = Loc.GetString("pontific-fel-alert", ("fel", pontificComponent.PontificFel));
        popup.PopupEntity(message, args.User, args.User);
    }
}

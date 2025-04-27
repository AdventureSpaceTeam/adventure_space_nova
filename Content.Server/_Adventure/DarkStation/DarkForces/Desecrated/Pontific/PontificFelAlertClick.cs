using Content.Server.Popups;
using Content.Shared.Alert;

namespace Content.Server._Adventure.DarkForces.Desecrated.Pontific;

[DataDefinition]
public sealed partial class PontificFelAlertClick : IAlertClick
{
    public void AlertClicked(EntityUid player)
    {
        var entManager = IoCManager.Resolve<IEntityManager>();
        if (!entManager.TryGetComponent<PontificComponent>(player, out var pontificComponent))
            return;

        var popup = entManager.System<PopupSystem>();
        var message = Loc.GetString("pontific-fel-alert", ("fel", pontificComponent.PontificFel));
        popup.PopupEntity(message, player, player);
    }
}

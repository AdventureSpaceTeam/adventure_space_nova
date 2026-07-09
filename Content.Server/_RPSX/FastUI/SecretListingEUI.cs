using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.RPSX.FastUI;
using JetBrains.Annotations;
using Robust.Shared.Player;
using static Content.Shared.FastUI.SecretListingEUIState;

namespace Content.Server.RPSX.FastUI;

[UsedImplicitly]
public sealed class SecretListingEUI : BaseEui
{
    public override void HandleMessage(EuiMessageBase msg)
    {
        if (msg is not SelectItemEUIMessage data)
            return;

        var entityManager = IoCManager.Resolve<IEntityManager>();
        var entityUid = entityManager.GetEntity(data.NetEntity);
        if (entityUid == EntityUid.Invalid)
        {
            entityManager.EventBus.RaiseEvent(EventSource.Local, new SecretListingEUISelectedEvent(data.Key, data.Data));
            return;
        }
        entityManager.EventBus.RaiseLocalEvent(entityUid, new SecretListingEUISelectedEvent(data.Key, data.Data));
    }
    public static SecretListingEUI ShowSecretListingEUI(IEntityManager entityManager, ICommonSession player, SecretListingCategoryPrototype prototype, bool global)
    {
        var eui = IoCManager.Resolve<EuiManager>();
        var ui = new SecretListingEUI();

        eui.OpenEui(ui, player);

        var entityUid = global ? EntityUid.Invalid : player.AttachedEntity ?? EntityUid.Invalid;
        ui.SendMessage(new SecretListingEUIInitState(prototype, entityManager.GetNetEntity(entityUid)));

        return ui;
    }
}

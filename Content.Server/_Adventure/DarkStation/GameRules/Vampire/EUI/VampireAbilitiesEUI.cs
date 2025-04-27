using Content.Server.AdventureSpace.GameRules.Vampire.Role.Events;
using Content.Server.EUI;
using Content.Shared.AdventureSpace.DarkForces.Vampire;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Server.AdventureSpace.GameRules.Vampire.EUI;

[UsedImplicitly]
public sealed class VampireAbilitiesEUI : BaseEui
{
    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is not VampireAbilitySelected data)
            return;

        var entityManager = IoCManager.Resolve<IEntityManager>();
        var entityUid = entityManager.GetEntity(data.NetEntity);
        var ev = new VampireAbilitySelectedEvent(data.Action, data.BloodRequired, data.ReplaceId);

        entityManager.EventBus.RaiseLocalEvent(entityUid, ref ev);
        Close();
    }
}

using Content.Server.RPSX.DarkForces.Vampire.Role.Events;
using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.RPSX.DarkForces.Vampire;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using JetBrains.Annotations;

namespace Content.Server.RPSX.DarkForces.Vampire.EUI;

[UsedImplicitly]
public sealed class VampireAbilitiesEUI : BaseEui
{
    [Dependency] private readonly EntityManager _entityManager = default!;
    private readonly EntityUid _owner;

    public VampireAbilitiesEUI(EntityUid owner)
    {
        IoCManager.InjectDependencies(this);
        _owner = owner;
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is not VampireAbilitySelected data)
            return;

        var entityUid = _entityManager.GetEntity(data.NetEntity);
        var ev = new VampireAbilitySelectedEvent(data.Action, data.BloodRequired, data.ReplaceId);

        _entityManager.EventBus.RaiseLocalEvent(entityUid, ref ev);
        Close();
    }

    public override void Closed()
    {
        base.Closed();

        if (_entityManager.EntityExists(_owner) && _entityManager.TryGetComponent<VampireComponent>(_owner, out var comp))
            comp.AbilitiesUiOpen = false;
    }
}

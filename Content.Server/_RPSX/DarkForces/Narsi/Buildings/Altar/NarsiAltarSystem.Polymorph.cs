using Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;
using Content.Shared.Polymorph;

namespace Content.Server.RPSX.DarkForces.Narsi.Buildings.Altar;

public sealed partial class NarsiAltarSystem
{
    private void InitPolymorph()
    {
        SubscribeLocalEvent<NarsiAltarComponent, NarsiRequestPolymorphEvent>(OnPolymorphRequest);
        SubscribeLocalEvent<NarsiPolymorphComponent, PolymorphedEvent>(OnPolymorphReverted);
    }

    private void OnPolymorphRequest(EntityUid uid, NarsiAltarComponent component, NarsiRequestPolymorphEvent args)
    {
        var polymorphEntity = _polymorph.PolymorphEntity(args.Target, args.Configuration);
        if (polymorphEntity == null)
            return;

        var polymorphComponent = EnsureComp<NarsiPolymorphComponent>(polymorphEntity.Value);
        polymorphComponent.AltarEntityUid = uid;
        polymorphComponent.ReturnToAltar = args.ReturnToAltar;
    }

    private void OnPolymorphReverted(EntityUid uid, NarsiPolymorphComponent component, PolymorphedEvent args)
    {
        if (!args.IsRevert)
            return;

        var altar = component.AltarEntityUid;
        if (!component.ReturnToAltar || !EntityManager.EntityExists(altar))
            return;

        var transform = Transform(altar);
        _transform.SetCoordinates(args.OldEntity, transform.Coordinates);
    }
}

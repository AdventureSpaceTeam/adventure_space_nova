using System.Linq;
using Content.Server.Body.Events;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.Medical.Surgery.Organs.Stomach;

public sealed class SurgeryStomachSystem : SurgeryOrganSystem<DStomachComponent, StomachMarkerComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DStomachComponent, OnEntityStomachUpdated>(OnStomachUpdated);
    }

    private void OnStomachUpdated(Entity<DStomachComponent> ent, ref OnEntityStomachUpdated args)
    {
        if (!_prototype.TryIndex<ReagentPrototype>(args.Quantity.Reagent.Prototype, out var proto))
            return;

        var toxinFound = proto.Metabolisms
            ?.Keys.Any(meta => ent.Comp.ToxinsGroups.Contains(meta)) ?? false;

        if (!toxinFound)
            return;

        ent.Comp.Damage += args.Quantity.Quantity.Float();
        UpdateOrganStatus(ent);
    }
}

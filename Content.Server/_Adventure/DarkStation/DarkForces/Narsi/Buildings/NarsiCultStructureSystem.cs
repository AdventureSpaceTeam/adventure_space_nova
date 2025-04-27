using Content.Server._Adventure.DarkForces.Narsi.Progress.Objectives.Building;

namespace Content.Server._Adventure.DarkForces.Narsi.Buildings;

public sealed class NarsiCultStructureSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NarsiCultStructureComponent, ComponentInit>(OnInit);
    }

    private void OnInit(EntityUid uid, NarsiCultStructureComponent component, ComponentInit args)
    {
        RaiseLocalEvent(new NarsiBuildingEvent(component.Building));
    }
}

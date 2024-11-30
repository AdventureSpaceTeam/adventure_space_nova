using Content.Shared.Polymorph;

namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Buildings.Altar.Rituals.Polymorth;

public sealed class NarsiRequestPolymorphEvent : CancellableEntityEventArgs
{
    public PolymorphConfiguration Configuration;
    public bool ReturnToAltar;
    public EntityUid Target;


    public NarsiRequestPolymorphEvent(EntityUid target, PolymorphConfiguration configuration, bool returnToAltar)
    {
        Target = target;
        Configuration = configuration;
        ReturnToAltar = returnToAltar;
    }
}

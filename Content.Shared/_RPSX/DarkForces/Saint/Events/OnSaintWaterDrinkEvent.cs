namespace Content.Shared.RPSX.DarkForces.Saint.Reagent.Events;

/**
 * Событие прокидывается, когда святая вода выпита.
 * Может быть перехвачено системами или отменено
 */
public sealed class OnSaintWaterDrinkEvent : CancellableEntityEventArgs
{
    public EntityUid Target;
    public OnSaintWaterDrinkEvent(EntityUid target)
    {
        Target = target;
    }
}

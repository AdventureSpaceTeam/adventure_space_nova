using Content.Shared.FixedPoint;

namespace Content.Server.AdventureSpace.DarkForces.Saint.Reagent.Events;

/**
 * Событие прокидывается, когда святая вода выпита.
 * Может быть перехвачено системами или отменено
 */
public sealed class OnSaintWaterDrinkEvent : CancellableEntityEventArgs
{
    public FixedPoint2 SaintWaterAmount;
    public EntityUid Target;

    public OnSaintWaterDrinkEvent(EntityUid target, FixedPoint2 saintWaterAmount)
    {
        Target = target;
        SaintWaterAmount = saintWaterAmount;
    }
}

using Content.Shared.Chemistry;
using Content.Shared.FixedPoint;

namespace Content.Server.AdventureSpace.DarkForces.Saint.Reagent.Events;

/**
 * Вызывается, если у сущности есть Flammable в Reactive компоненте
 */
public sealed class OnSaintWaterFlammableEvent : CancellableEntityEventArgs
{
    public ReactionMethod? ReactionMethod;
    public FixedPoint2 SaintWaterAmount;
    public EntityUid Target;

    public OnSaintWaterFlammableEvent(EntityUid target, FixedPoint2 saintWaterAmount, ReactionMethod? reactionMethod)
    {
        Target = target;
        SaintWaterAmount = saintWaterAmount;
        ReactionMethod = reactionMethod;
    }
}

namespace Content.Shared.RPSX.DarkForces.Saint.Reagent.Events;

/**
 * Вызывается, если у сущности есть Flammable в Reactive компоненте
 */
public sealed class OnSaintWaterFlammableEvent : CancellableEntityEventArgs
{
    public EntityUid Target;
    public OnSaintWaterFlammableEvent(EntityUid target)
    {
        Target = target;
    }
}

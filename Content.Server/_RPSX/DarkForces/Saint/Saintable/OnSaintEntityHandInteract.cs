using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.DarkForces.Saint.Saintable.Events;

public sealed class OnSaintEntityHandInteract : HandledEntityEventArgs, ISaintEntityEvent
{
    public DamageSpecifier DamageOnCollide { get; set; }
    public bool PushOnCollide { get; set; }
    public bool IsHandled
    {
        get => Handled;
        set => Handled = value;
    }

    public OnSaintEntityHandInteract(DamageSpecifier damageOnCollide, bool pushOnCollide)
    {
        DamageOnCollide = damageOnCollide;
        PushOnCollide = pushOnCollide;
    }
}

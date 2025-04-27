using Content.Shared.Damage;

namespace Content.Server.AdventureSpace.DarkForces.Saint.Saintable.Events;

public sealed class OnSilverEntityHandInteract : HandledEntityEventArgs, ISaintEntityEvent
{
    public OnSilverEntityHandInteract(DamageSpecifier damageOnCollide, bool pushOnCollide)
    {
        DamageOnCollide = damageOnCollide;
        PushOnCollide = pushOnCollide;
    }

    public DamageSpecifier DamageOnCollide { get; set; }
    public bool PushOnCollide { get; set; }

    public bool IsHandled
    {
        get => Handled;
        set => Handled = value;
    }
}

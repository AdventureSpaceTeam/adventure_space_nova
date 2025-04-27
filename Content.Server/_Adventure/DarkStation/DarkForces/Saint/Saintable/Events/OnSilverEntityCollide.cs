using Content.Shared.Damage;

namespace Content.Server._Adventure.DarkForces.Saint.Saintable.Events;

public sealed class OnSilverEntityCollide : HandledEntityEventArgs, ISaintEntityEvent
{
    public OnSilverEntityCollide(DamageSpecifier damageOnCollide, bool pushOnCollide)
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

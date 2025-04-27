using Content.Shared.Damage;

namespace Content.Server.AdventureSpace.DarkForces.Saint.Saintable.Events;

public interface ISaintEntityEvent
{
    public DamageSpecifier DamageOnCollide { get; set; }
    public bool PushOnCollide { get; set; }

    public bool IsHandled { get; set; }
}

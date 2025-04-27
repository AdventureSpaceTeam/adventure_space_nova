using Content.Shared._Adventure.SCP.SCP_049.Components;
using Content.Shared.Weapons.Ranged.Events;
using SCP049Component = Content.Shared._Adventure.SCP.SCP_049.Components.SCP049Component;

namespace Content.Shared._Adventure.SCP.SCP_049;

public abstract partial class SharedSCP049System : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCP049Component, ShotAttemptedEvent>(OnShootAttempt);
    }

    private void OnShootAttempt(Entity<SCP049Component> ent, ref ShotAttemptedEvent args)
    {
        args.Cancel();
    }
}

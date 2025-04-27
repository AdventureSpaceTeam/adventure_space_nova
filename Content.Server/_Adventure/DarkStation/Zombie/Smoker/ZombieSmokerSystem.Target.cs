using Content.Shared.AdventureSpace.Zombie.Smoker.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Zombies;
using Robust.Shared.Player;

namespace Content.Server.AdventureSpace.Zombie.Smoker;

public sealed partial class ZombieSmokerSystem
{
    private void InitializeTarget()
    {
        SubscribeLocalEvent<ZombieSmokerTargetComponent, ComponentInit>(OnTargetInit);
        SubscribeLocalEvent<ZombieSmokerTargetComponent, ComponentShutdown>(OnTargetShutdown);
        SubscribeLocalEvent<ZombieSmokerTargetComponent, MobStateChangedEvent>(OnTargetStateChanged);
    }

    private void OnTargetInit(EntityUid uid, ZombieSmokerTargetComponent component, ComponentInit args)
    {
        _blockerSystem.UpdateCanMove(uid);
        UpdateTargetAppearance(uid, true);
        _audioSystem.PlayEntity(component.Sound, Filter.Pvs(uid, entityManager: EntityManager), uid, true);
    }

    private void OnTargetShutdown(EntityUid uid, ZombieSmokerTargetComponent component, ComponentShutdown args)
    {
        _blockerSystem.UpdateCanMove(uid);
        UpdateTargetAppearance(uid, false);
    }

    private void OnTargetStateChanged(EntityUid uid, ZombieSmokerTargetComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead || component.Smoker == EntityUid.Invalid)
            return;

        if (!TryComp<ZombieSmokerComponent>(component.Smoker, out var smoker))
            return;

        DropSmoker((component.Smoker, smoker));
    }

    private bool IsTargetValid(EntityUid target)
    {
        return !_mobStateSystem.IsDead(target)
               && HasComp<HumanoidAppearanceComponent>(target)
               && !HasComp<ZombieComponent>(target);
    }

    private void UpdateTargetAppearance(EntityUid target, bool isCuffed)
    {
        if (!TryComp<AppearanceComponent>(target, out var appearance))
            return;

        _appearanceSystem.SetData(target, ZombieSmokerVisuals.Cuffed, isCuffed, appearance);
    }
}

using Content.Server.Anomaly.Components;
using Content.Shared.Anomaly;
using Robust.Shared.Physics.Events;
using SCPAnomalyGunGoalComponent =
    Content.Server._Adventure.GameRules.SCP.Station.Goals.Components.SCPAnomalyGunGoalComponent;

namespace Content.Server._Adventure.GameRules.SCP.Station.Goals.SubSystems;

public sealed class SCPAnomalyGunSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCPAnomalyGunGoalComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SCPAnomalyGunGoalComponent, StartCollideEvent>(OnStartCollide);
    }

    private void OnComponentInit(Entity<SCPAnomalyGunGoalComponent> ent, ref ComponentInit args)
    {
        ent.Comp.RequirementParticles.Add(AnomalousParticleType.Delta);
        ent.Comp.RequirementParticles.Add(AnomalousParticleType.Epsilon);
        ent.Comp.RequirementParticles.Add(AnomalousParticleType.Zeta);
        ent.Comp.RequirementParticles.Add(AnomalousParticleType.Sigma);
    }

    private void OnStartCollide(EntityUid uid, SCPAnomalyGunGoalComponent component, ref StartCollideEvent args)
    {
        if (!TryComp<AnomalousParticleComponent>(args.OtherEntity, out var particle))
            return;

        if (!SCPGoalUtils.IsSCPOnMainStation(uid, EntityManager))
            return;

        component.CurrentPatrticles.Add(particle.ParticleType);

        if (component.CurrentPatrticles.SetEquals(component.RequirementParticles))
            SCPGoalUtils.MakeTaskCompleted(uid, EntityManager);
    }
}

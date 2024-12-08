using Content.Shared.Anomaly;

namespace Content.Server.AdventurePrivate._Alteros.GameRules.SCP.Station.Goals.Components;

[RegisterComponent]
public sealed partial class SCPAnomalyGunGoalComponent : Component
{
    [DataField]
    public HashSet<AnomalousParticleType> CurrentPatrticles = []!;

    [DataField]
    public HashSet<AnomalousParticleType> RequirementParticles = [];
}

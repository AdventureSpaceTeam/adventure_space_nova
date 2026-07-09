using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress.Objectives.Rituals;

[ByRefEvent]
public record struct NarsiRitualCompletedEvent(string Ritual);

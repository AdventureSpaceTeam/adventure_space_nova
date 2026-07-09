using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Events;

[ByRefEvent]
public record VampireAbilitySelectedEvent(string ActionId, int BloodRequired, EntProtoId? ReplaceId);

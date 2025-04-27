using Robust.Shared.Prototypes;

namespace Content.Server.AdventureSpace.GameRules.Vampire.Role.Events;

[ByRefEvent]
public record VampireAbilitySelectedEvent(string ActionId, int BloodRequired, EntProtoId? ReplaceId);

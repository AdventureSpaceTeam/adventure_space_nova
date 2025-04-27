using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.GameRules.Vampire.Role.Events;

[ByRefEvent]
public record VampireAbilitySelectedEvent(string ActionId, int BloodRequired, EntProtoId? ReplaceId);

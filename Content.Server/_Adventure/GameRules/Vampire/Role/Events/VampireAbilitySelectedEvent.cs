using Robust.Shared.Prototypes;

namespace Content.Server.AdventurePrivate._Alteros.GameRules.Vampire.Role.Events;

[ByRefEvent]
public record VampireAbilitySelectedEvent(string ActionId, int BloodRequired, EntProtoId? ReplaceId);

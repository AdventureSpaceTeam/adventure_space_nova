using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.DarkForces.Narsi.Buildings.Altar.Buildings;

[Serializable, NetSerializable]
public record NarsiBuildingsState(List<NarsiBuildingUIModel> LearnedBuildings, List<NarsiBuildingUIModel> ToLearnBuildings);

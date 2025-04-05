using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions
/// <summary>
/// Реакция сгорания водорода в кислородной среде.
/// </summary>
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class HydrogenBurnReaction : IGasReactionEffect
    {
        public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
        {
            var energyReleased = 0f;
            var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            var temperature = mixture.Temperature;
            var location = holder as TileAtmosphere;

            mixture.ReactionResults[(byte)GasReaction.Fire] = 0f;

            var initialHydrogen = mixture.GetMoles(Gas.Hydrogen);
            var initialOxygen = mixture.GetMoles(Gas.Oxygen);

            // Минимальное соотношение H2:O2 = 2:1 (по стехиометрии)
            var burnRatio = Atmospherics.HydrogenBurnRate;

            var burnedFuel = Math.Min(
                initialHydrogen * burnRatio,
                initialOxygen * Atmospherics.HydrogenBurnOxyFactor // 1 моль O2 требуется на 2 моля H2
            );

            if (burnedFuel <= 0f)
                return ReactionResult.NoReaction;

            mixture.AdjustMoles(Gas.Hydrogen, -burnedFuel);
            mixture.AdjustMoles(Gas.Oxygen, -burnedFuel * 0.5f);

            energyReleased += burnedFuel * Atmospherics.FireHydrogenEnergyReleased;

            mixture.ReactionResults[(byte)GasReaction.Fire] += burnedFuel;

            energyReleased /= heatScale;

            if (energyReleased > 0)
            {
                var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
                if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
                    mixture.Temperature = ((temperature * oldHeatCapacity + energyReleased) / newHeatCapacity);
            }

            if (location != null && mixture.Temperature > Atmospherics.FireMinimumTemperatureToExist)
            {
                atmosphereSystem.HotspotExpose(location, mixture.Temperature, mixture.Volume);
            }

            return ReactionResult.Reacting;
        }
    }
}

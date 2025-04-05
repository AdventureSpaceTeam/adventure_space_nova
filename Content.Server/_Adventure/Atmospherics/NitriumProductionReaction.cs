using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class NitriumProductionReaction : IGasReactionEffect
    {
        public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
        {
            var initialTritium = mixture.GetMoles(Gas.Tritium);
            var initialNitrogen = mixture.GetMoles(Gas.Nitrogen);
            var initialBZ = mixture.GetMoles(Gas.BZ);

            if (initialTritium < Atmospherics.NitriumMinTritium ||
                initialNitrogen < Atmospherics.NitriumMinNitrogen ||
                initialBZ < Atmospherics.NitriumMinBZ ||
                mixture.Temperature < Atmospherics.NitriumProductionMinTemp)
            {
                return ReactionResult.NoReaction;
            }

            var productionEfficiency = Math.Min(
                mixture.Temperature / Atmospherics.NitriumProductionTempDivisor,
                Math.Min(
                    initialTritium / Atmospherics.NitriumTritiumRatio,
                    Math.Min(
                        initialNitrogen / Atmospherics.NitriumNitrogenRatio,
                        initialBZ / Atmospherics.NitriumBZRatio
                    )
                )
            );

            productionEfficiency = Math.Min(productionEfficiency, Atmospherics.NitriumProductionMaxRate);

            if (productionEfficiency <= 0f ||
                initialTritium - productionEfficiency * Atmospherics.NitriumTritiumRatio < 0f ||
                initialNitrogen - productionEfficiency * Atmospherics.NitriumNitrogenRatio < 0f ||
                initialBZ - productionEfficiency * Atmospherics.NitriumBZRatio < 0f)
            {
                return ReactionResult.NoReaction;
            }

            var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            var temperature = mixture.Temperature;

            mixture.AdjustMoles(Gas.Tritium, -productionEfficiency * Atmospherics.NitriumTritiumRatio);
            mixture.AdjustMoles(Gas.Nitrogen, -productionEfficiency * Atmospherics.NitriumNitrogenRatio);
            mixture.AdjustMoles(Gas.BZ, -productionEfficiency * Atmospherics.NitriumBZRatio);
            mixture.AdjustMoles(Gas.Nitrium, productionEfficiency);

            var energyConsumed = productionEfficiency * Atmospherics.NitriumProductionEnergy / heatScale;

            if (energyConsumed > 0f)
            {
                var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
                if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
                {
                    mixture.Temperature = Math.Max(
                        (temperature * oldHeatCapacity - energyConsumed) / newHeatCapacity,
                        Atmospherics.TCMB
                    );
                }
            }

            return ReactionResult.Reacting;
        }
    }
}

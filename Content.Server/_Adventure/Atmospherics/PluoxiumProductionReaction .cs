using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
/// Реакция синтеза плюоксиума из диоксида, кислорода и трития.
/// Соотношение: 1 CO2 + 0.5 O2 + 0.01 трития → 1 плюоксиума + 0.01 H2 + Энергия
/// </summary>
[UsedImplicitly]
[DataDefinition]
public sealed partial class PluoxiumProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var initialCO2 = mixture.GetMoles(Gas.CarbonDioxide);
        var initialO2 = mixture.GetMoles(Gas.Oxygen);
        var initialTritium = mixture.GetMoles(Gas.Tritium);
        var producedAmount = Math.Min(
            Atmospherics.PluoxiumFormationMaxRate,
            Math.Min(
                initialCO2,
                Math.Min(
                    initialO2 / Atmospherics.PluoxiumOxygenRatio,
                    initialTritium / Atmospherics.PluoxiumTritiumRatio
                )
            )
        );
        if (producedAmount <= 0 ||
            initialCO2 - producedAmount < 0 ||
            initialO2 - producedAmount * Atmospherics.PluoxiumOxygenRatio < 0 ||
            initialTritium - producedAmount * Atmospherics.PluoxiumTritiumRatio < 0)
        {
            return ReactionResult.NoReaction;
        }
        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        var temperature = mixture.Temperature;
        mixture.AdjustMoles(Gas.CarbonDioxide, -producedAmount);
        mixture.AdjustMoles(Gas.Oxygen, -producedAmount * Atmospherics.PluoxiumOxygenRatio);
        mixture.AdjustMoles(Gas.Tritium, -producedAmount * Atmospherics.PluoxiumTritiumRatio);
        mixture.AdjustMoles(Gas.Pluoxium, producedAmount);
        mixture.AdjustMoles(Gas.Hydrogen, producedAmount * Atmospherics.PluoxiumHydrogenByproductRatio);
        var energyReleased = producedAmount * Atmospherics.PluoxiumFormationEnergy / heatScale;
        if (energyReleased > 0)
        {
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature = Math.Max(
                    (temperature * oldHeatCapacity + energyReleased) / newHeatCapacity,
                    Atmospherics.TCMB
                );
            }
        }
        return ReactionResult.Reacting;
    }
}

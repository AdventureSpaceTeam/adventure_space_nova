using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;
/// <summary>
/// Реакция поглощения кислорода и тепла галоном при нагревании.
/// </summary>
[UsedImplicitly]
public sealed partial class HalonFireSuppressionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature < Atmospherics.HalonActivationTemperature)
            return ReactionResult.NoReaction;

        var initialHalon = mixture.GetMoles(Gas.Halon);
        var initialOxygen = mixture.GetMoles(Gas.Oxygen);

        if (initialHalon <= 0 || initialOxygen <= 0)
            return ReactionResult.NoReaction;

        var temperatureFactor = MathHelper.Clamp(
            (mixture.Temperature - Atmospherics.HalonActivationTemperature) /
            (Atmospherics.HalonMaxTemperature - Atmospherics.HalonActivationTemperature),
            0f, 1f);

        var absorptionRate = initialHalon * temperatureFactor * Atmospherics.HalonAbsorptionRate;

        var oxygenAbsorbed = Math.Min(absorptionRate, initialOxygen);
        var heatAbsorbed = oxygenAbsorbed * Atmospherics.HalonHeatAbsorptionFactor;

        mixture.AdjustMoles(Gas.Halon, -oxygenAbsorbed * 0.1f);
        mixture.AdjustMoles(Gas.Oxygen, -oxygenAbsorbed);
        mixture.AdjustMoles(Gas.CarbonDioxide, oxygenAbsorbed * 0.8f);

        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature -= heatAbsorbed / heatCapacity;

        return ReactionResult.Reacting;
    }
}

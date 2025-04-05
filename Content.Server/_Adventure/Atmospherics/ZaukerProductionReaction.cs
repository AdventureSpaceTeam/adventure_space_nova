using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Производство заукера из гипер-ноблиума и нитриума.
///     Требует экстремально высоких температур (50000K-75000K).
///     Максимальное производство ограничено 5 молями за тик.
/// </summary>
[UsedImplicitly]
public sealed partial class ZaukerProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature < Atmospherics.ZaukerProductionMinTemperature ||
            mixture.Temperature > Atmospherics.ZaukerProductionMaxTemperature)
        {
            return ReactionResult.NoReaction;
        }

        var initialHyperNob = mixture.GetMoles(Gas.HyperNoblium);
        var initialNitrium = mixture.GetMoles(Gas.Nitrium);

        if (initialHyperNob <= 0 || initialNitrium <= 0)
            return ReactionResult.NoReaction;

        var temperatureFactor = mixture.Temperature * Atmospherics.ZaukerProductionTemperatureScale;

        var hyperNobLimit = initialHyperNob / Atmospherics.ZaukerHyperNobRatio;
        var nitriumLimit = initialNitrium / Atmospherics.ZaukerNitriumRatio;
        var limitingFactor = Math.Min(hyperNobLimit, nitriumLimit);

        if (limitingFactor <= 0)
            return ReactionResult.NoReaction;

        var reactionRate = Math.Min(limitingFactor, temperatureFactor);

        var hyperNobBurned = reactionRate * Atmospherics.ZaukerHyperNobRatio;
        var nitriumBurned = reactionRate * Atmospherics.ZaukerNitriumRatio;

        var zaukerProduced = (hyperNobBurned + nitriumBurned) * Atmospherics.ZaukerProductionEfficiency;

        zaukerProduced = Math.Min(zaukerProduced, Atmospherics.ZaukerProductionMaxPerTick);

        var efficiencyRatio = zaukerProduced / ((hyperNobBurned + nitriumBurned) * Atmospherics.ZaukerProductionEfficiency);
        hyperNobBurned *= efficiencyRatio;
        nitriumBurned *= efficiencyRatio;

        mixture.AdjustMoles(Gas.HyperNoblium, -hyperNobBurned);
        mixture.AdjustMoles(Gas.Nitrium, -nitriumBurned);
        mixture.AdjustMoles(Gas.Zauker, zaukerProduced);

        var energyConsumed = zaukerProduced * Atmospherics.ZaukerProductionEnergy / heatScale;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature = Math.Max(mixture.Temperature - energyConsumed / heatCapacity, Atmospherics.TCMB);

        return ReactionResult.Reacting;
    }
}

using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Производство прото-нитрата из плюоксиума и водорода.
///     Требует высоких температур (5000K-10000K) и имеет температурный коэффициент.
/// </summary>
[UsedImplicitly]
public sealed partial class ProtoNitrateProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature < Atmospherics.PNProductionMinTemperature ||
            mixture.Temperature > Atmospherics.PNProductionMaxTemperature)
        {
            return ReactionResult.NoReaction;
        }

        var initialPluoxium = mixture.GetMoles(Gas.Pluoxium);
        var initialHydrogen = mixture.GetMoles(Gas.Hydrogen);

        if (initialPluoxium <= 0 || initialHydrogen <= 0)
            return ReactionResult.NoReaction;

        var temperatureFactor = mixture.Temperature * Atmospherics.PNProductionTemperatureScale;

        var pluoxiumLimit = initialPluoxium / Atmospherics.PNPluoxiumRatio;
        var hydrogenLimit = initialHydrogen / Atmospherics.PNHydrogenRatio;
        var limitingFactor = Math.Min(pluoxiumLimit, hydrogenLimit);

        if (limitingFactor <= 0)
            return ReactionResult.NoReaction;

        var reactionRate = Math.Min(limitingFactor, temperatureFactor);

        var pluoxiumBurned = reactionRate * Atmospherics.PNPluoxiumRatio;
        var hydrogenBurned = reactionRate * Atmospherics.PNHydrogenRatio;

        var protoNitrateProduced = (pluoxiumBurned + hydrogenBurned) * Atmospherics.PNProductionEfficiency;

        mixture.AdjustMoles(Gas.Pluoxium, -pluoxiumBurned);
        mixture.AdjustMoles(Gas.Hydrogen, -hydrogenBurned);
        mixture.AdjustMoles(Gas.ProtoNitrate, protoNitrateProduced);

        var energyToAdd = protoNitrateProduced * Atmospherics.PNProductionEnergy / heatScale;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature += energyToAdd / heatCapacity;

        return ReactionResult.Reacting;
    }
}

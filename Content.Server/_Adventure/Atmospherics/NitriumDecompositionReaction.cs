using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Разложение нитриума в кислородной среде.
/// </summary>
[UsedImplicitly]
public sealed partial class NitriumDecompositionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature > Atmospherics.NitriumDecompositionMaxTemp)
            return ReactionResult.NoReaction;

        var initialNitrium = mixture.GetMoles(Gas.Nitrium);
        if (initialNitrium <= 0)
            return ReactionResult.NoReaction;

        var efficiency = Math.Min(
            mixture.Temperature / Atmospherics.NitriumDecompositionTempDivisor,
            initialNitrium
        );

        var decomposedAmount = efficiency * Atmospherics.NitriumDecompositionRate;
        mixture.AdjustMoles(Gas.Nitrium, -decomposedAmount);
        mixture.AdjustMoles(Gas.Nitrogen, decomposedAmount);
        mixture.AdjustMoles(Gas.Hydrogen, decomposedAmount);

        var energyReleased = decomposedAmount * Atmospherics.NitriumDecompositionEnergy;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature += energyReleased / heatCapacity;

        return ReactionResult.Reacting;
    }
}

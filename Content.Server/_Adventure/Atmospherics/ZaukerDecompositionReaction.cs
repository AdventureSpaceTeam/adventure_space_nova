using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Разложение заукера в азотной среде.
/// </summary>
[UsedImplicitly]
public sealed partial class ZaukerDecompositionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var initialZauker = mixture.GetMoles(Gas.Zauker);
        var initialNitrogen = mixture.GetMoles(Gas.Nitrogen);

        if (initialZauker <= 0 || initialNitrogen <= 0)
            return ReactionResult.NoReaction;

        var decomposedAmount = Math.Min(
            Math.Min(initialZauker, initialNitrogen),
            Atmospherics.ZaukerDecompositionMaxRate
        );

        mixture.AdjustMoles(Gas.Zauker, -decomposedAmount);
        mixture.AdjustMoles(Gas.Nitrogen, decomposedAmount * 0.7f);
        mixture.AdjustMoles(Gas.Oxygen, decomposedAmount * 0.3f);

        var energyReleased = decomposedAmount * Atmospherics.ZaukerDecompositionEnergy;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature += energyReleased / heatCapacity;

        return ReactionResult.Reacting;
    }
}

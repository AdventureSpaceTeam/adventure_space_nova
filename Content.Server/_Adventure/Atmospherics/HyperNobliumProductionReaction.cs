using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;
/// <summary>
/// Реакция синтеза гипер-ноблиума из азота и трития
/// Соотношение: 10 азота + 5 трития → 1 гипер-ноблиума
/// </summary>
[UsedImplicitly]
public sealed partial class HyperNobliumProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature < Atmospherics.HyperNobliumProductionMinTemp ||
            mixture.Temperature > Atmospherics.HyperNobliumProductionMaxTemp)
            return ReactionResult.NoReaction;

        var initialNitrogen = mixture.GetMoles(Gas.Nitrogen);
        var initialTritium = mixture.GetMoles(Gas.Tritium);
        var initialBZ = mixture.GetMoles(Gas.BZ);

        var totalGas = initialTritium + initialBZ;
        var tritiumReductionFactor = totalGas > 0
            ? Math.Clamp(initialTritium / totalGas, 0.001f, 1f)
            : 1f;

        var nobliumPossible = Math.Min(
            (initialNitrogen + initialTritium) * 0.01f,
            Math.Min(
                initialTritium / (Atmospherics.HyperNobliumProductionTritiumRatio * tritiumReductionFactor),
                initialNitrogen / Atmospherics.HyperNobliumProductionNitrogenRatio
            )
        );

        nobliumPossible = Math.Min(nobliumPossible, Atmospherics.HyperNobliumProductionMaxRate);

        if (nobliumPossible <= 0 ||
            initialTritium < Atmospherics.HyperNobliumProductionTritiumRatio * nobliumPossible * tritiumReductionFactor ||
            initialNitrogen < Atmospherics.HyperNobliumProductionNitrogenRatio * nobliumPossible)
        {
            return ReactionResult.NoReaction;
        }

        mixture.AdjustMoles(Gas.Nitrogen, -Atmospherics.HyperNobliumProductionNitrogenRatio * nobliumPossible);
        mixture.AdjustMoles(Gas.Tritium, -Atmospherics.HyperNobliumProductionTritiumRatio * nobliumPossible * tritiumReductionFactor);
        mixture.AdjustMoles(Gas.HyperNoblium, nobliumPossible);

        var energyReleased = nobliumPossible *
                           (Atmospherics.HyperNobliumProductionEnergy /
                           Math.Max(initialBZ, 1f)) / heatScale;

        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
        {
            mixture.Temperature = Math.Max(
                (mixture.Temperature * heatCapacity + energyReleased) / heatCapacity,
                Atmospherics.TCMB
            );
        }

        return ReactionResult.Reacting;
    }
}

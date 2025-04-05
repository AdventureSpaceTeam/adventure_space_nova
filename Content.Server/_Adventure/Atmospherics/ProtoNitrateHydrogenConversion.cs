using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Конверсия водорода c участием прото-нитрата.
/// </summary>
[UsedImplicitly]
public sealed partial class ProtoNitrateHydrogenConversion : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var initialHydrogen = mixture.GetMoles(Gas.Hydrogen);
        var initialPN = mixture.GetMoles(Gas.ProtoNitrate);

        if (initialHydrogen < Atmospherics.PNHydrogenConversionThreshold || initialPN <= 0)
            return ReactionResult.NoReaction;

        var conversionRate = Math.Min(
            Math.Min(initialHydrogen, initialPN * Atmospherics.PNHydrogenConversionRate),
            Atmospherics.PNHydrogenConversionMaxRate
        );

        mixture.AdjustMoles(Gas.Hydrogen, -conversionRate);
        mixture.AdjustMoles(Gas.ProtoNitrate, conversionRate * 0.5f);

        var energyConsumed = conversionRate * Atmospherics.PNHydrogenConversionEnergy;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature = Math.Max(
                mixture.Temperature - energyConsumed / heatCapacity,
                Atmospherics.TCMB
            );

        return ReactionResult.Reacting;
    }
}

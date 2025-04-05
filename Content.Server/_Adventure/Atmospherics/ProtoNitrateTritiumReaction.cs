using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Детоксикация трития с помощью прото-нитрата.
/// </summary>
[UsedImplicitly]
public sealed partial class ProtoNitrateTritiumReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature < Atmospherics.PNTritiumMinTemp ||
            mixture.Temperature > Atmospherics.PNTritiumMaxTemp)
            return ReactionResult.NoReaction;

        var initialTritium = mixture.GetMoles(Gas.Tritium);
        var initialPN = mixture.GetMoles(Gas.ProtoNitrate);

        if (initialTritium <= 0 || initialPN <= 0)
            return ReactionResult.NoReaction;

        var reactionRate = Math.Min(
            initialTritium,
            initialPN * Atmospherics.PNTritiumConversionRate
        );

        mixture.AdjustMoles(Gas.Tritium, -reactionRate);
        mixture.AdjustMoles(Gas.Hydrogen, reactionRate);
        mixture.AdjustMoles(Gas.ProtoNitrate, -reactionRate * 0.01f);

        var energyReleased = reactionRate * Atmospherics.PNTritiumConversionEnergy;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature += energyReleased / heatCapacity;

        return ReactionResult.Reacting;
    }
}

using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Разложение БЗ с помощью прото-нитрата.
/// </summary>
[UsedImplicitly]
public sealed partial class ProtoNitrateBZDecomposition : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Temperature < Atmospherics.PNBZaseMinTemp ||
            mixture.Temperature > Atmospherics.PNBZaseMaxTemp)
            return ReactionResult.NoReaction;

        var initialBZ = mixture.GetMoles(Gas.BZ);
        var initialPN = mixture.GetMoles(Gas.ProtoNitrate);

        if (initialBZ <= 0 || initialPN <= 0)
            return ReactionResult.NoReaction;

        var reactionRate = Math.Min(
            initialBZ,
            initialPN * Atmospherics.PNBZaseConversionRate
        );

        mixture.AdjustMoles(Gas.BZ, -reactionRate);
        mixture.AdjustMoles(Gas.Nitrogen, reactionRate * 0.4f);
        mixture.AdjustMoles(Gas.Helium, reactionRate * 1.6f);
        mixture.AdjustMoles(Gas.Plasma, reactionRate * 0.8f);

        var energyReleased = reactionRate * Atmospherics.PNBZaseEnergy;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature += energyReleased / heatCapacity;

        return ReactionResult.Reacting;
    }
}

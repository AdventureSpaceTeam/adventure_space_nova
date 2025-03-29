using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
///     Синтез БЗ из плазмы и оксида азота.
///     Имеется лимит по давлению, если превышает 40КПа, реакция прекращается.
/// </summary>
[UsedImplicitly]
public sealed partial class BZProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Pressure > Atmospherics.BZSynthesisMaxPressure)
        {
            return ReactionResult.NoReaction;
        }
        var initialPlasma = mixture.GetMoles(Gas.Plasma);
        var initialN20 = mixture.GetMoles(Gas.NitrousOxide);

        if (initialPlasma <= 0 || initialN20 <= 0)
            return ReactionResult.NoReaction;

        var plasmaLimit = initialPlasma / Atmospherics.BZPlasmaRatio;
        var n20Limit = initialN20 / Atmospherics.BZN20Ratio;
        var limitingFactor = Math.Min(plasmaLimit, n20Limit);

        if (limitingFactor <= 0)
            return ReactionResult.NoReaction;

        limitingFactor = Math.Min(limitingFactor, Atmospherics.BZSynthesisMaxRate);

        var plasmaBurned = limitingFactor * Atmospherics.BZPlasmaRatio;
        var n20Burned = limitingFactor * Atmospherics.BZN20Ratio;
        var bzProduced = (plasmaBurned + n20Burned) * Atmospherics.BZSynthesisEfficiency;

        mixture.AdjustMoles(Gas.Plasma, -plasmaBurned);
        mixture.AdjustMoles(Gas.NitrousOxide, -n20Burned);
        mixture.AdjustMoles(Gas.BZ, bzProduced);

        var energyToAdd = bzProduced * Atmospherics.BZFormationEnergy / heatScale;
        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature += energyToAdd / heatCapacity;

        return ReactionResult.Reacting;
    }
}

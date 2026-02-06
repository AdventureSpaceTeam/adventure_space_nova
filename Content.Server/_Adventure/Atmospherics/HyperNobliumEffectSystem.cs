using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Adventure.Atmos.Reactions;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Adventure.Atmos.Reactions;

/// <summary>
/// Эффект газа гиперноблиум - полное подавление всех химических реакций при наличии ≥5 молей
/// </summary>
[UsedImplicitly]
public sealed partial class HypernobliumEffect : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var hyperNobliumMoles = mixture.GetMoles(Gas.HyperNoblium);
        var totalMoles = mixture.TotalMoles;

        if (totalMoles < Atmospherics.GasMinMoles)
            return ReactionResult.NoReaction;

        var fraction = hyperNobliumMoles / totalMoles;

        if (fraction >= Atmospherics.HyperNobliumFullSuppressionThresholdPercentage)
        {
            Array.Clear(mixture.ReactionResults, 0, mixture.ReactionResults.Length);
            return ReactionResult.StopReactions;
        }

        return ReactionResult.NoReaction;
    }
}


namespace Content.Shared.Atmos;

public static partial class Atmospherics
{
    /// <summary>
    ///     Defines energy released in N2O decomposition reaction.
    /// </summary>
    public const float NitrousOxideDecompositionEnergy = 200000f;

    /// <summary>
    ///     Defines energy released in Pluoxium formation.
    /// </summary>
    public const float PluoxiumFormationEnergy = 250f;

    /// <summary>
    ///     The maximum amount of pluoxium that can form per reaction tick.
    /// </summary>
    public const float PluoxiumMaxRate = 5f;
    public const float FireH2EnergyReleased = 2800000f;
    public const float H2OxygenFullBurn = 10f;
    public const float FireH2BurnRateDelta = 2f;
    public const float H2MinimumBurnTemperature = T0C + 100f;
    public const float NitriumFormationTempDivisor = (T0C + 100f) * 8f;
    public const float NitriumFormationEnergy = 100000f;
    public const float NitriumDecompositionTempDivisor = (T0C + 100f) * 8f;
    public const float NitriumDecompositionEnergy = 30000f;
    public const float NitriumDecompositionMaxTemp = T0C + 70f;
    public const float NobliumFormationEnergy = 20000000f;
    public const float ReactionOpperssionThreshold = 5f;
    public const float HalonFormationEnergy = 300f;
    public const float HalonCombustionEnergy = 2500f;
    public const float HealiumFormationEnergy = 9000f;
    public const float ZaukerFormationEnergy = 5000f;
    public const float ZaukerFormationTemperatureScale = 0.000005f;
    public const float ZaukerDecompositionMaxRate = 20f;
    public const float ZaukerDecompositionEnergy = 460f;
    public const float ProtoNitrateTemperatureScale = 0.005f;
    public const float ProtoNitrateFormationEnergy = 650f;
    public const float ProtoNitrateHydrogenConversionThreshold = 150f;
    public const float ProtoNitrateHydrogenConversionMaxRate = 5f;
    public const float ProtoNitrateHydrogenConversionEnergy = 2500f;
    public const float ProtoNitrateTritiumConversionEnergy = 10000f;
    public const float ProtoNitrateBZaseConversionEnergy = 60000f;
}

using Robust.Shared.Configuration;

namespace Content.Shared._Adventure.ADTCCVars;

[CVarDefs]
public sealed class ADTCCVars
{
    /// <summary>
    /// These variables control modifications of various gas prices. If gas has no specified
    /// modifier here, it will use default price from prototype
    /// </summary>

    public static readonly CVarDef<float> DefaultGasPriceModifier =
        CVarDef.Create("atmos.gas_price_modifier_default", 1f, CVar.SERVER);

    public static readonly CVarDef<float> GasPriceModifierTritium =
        CVarDef.Create("atmos.gas_price_modifier_tritium", 2.5f, CVar.SERVER);

    public static readonly CVarDef<float> GasPriceModifierNitrousOxide =
        CVarDef.Create("atmos.gas_price_modifier_nitrous_oxide", 0.1f, CVar.SERVER);

    public static readonly CVarDef<float> GasPriceModifierFrezon =
        CVarDef.Create("atmos.gas_price_modifier_frezon", 1f, CVar.SERVER);
}

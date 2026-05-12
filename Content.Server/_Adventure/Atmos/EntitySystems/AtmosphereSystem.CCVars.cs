using Content.Shared._Adventure.ADTCCVars;
using Robust.Shared.Configuration;

namespace Content.Server.Atmos.EntitySystems;

public enum GasIds
{
    Tritium,
    NitrousOxide,
    Frezon,
    BZ,
    Healium,
    Nitrium
}

public partial class AtmosphereSystem
{
    private float _defaultGasPriceModifier;
    private float _gasPriceModifierTritium;
    private float _gasPriceModifierNitrousOxide;
    private float _gasPriceModifierFrezon;

    private IDisposable? _configSub;

    public void InitADTAtmosCVars()
    {
        if (_configSub is not null)
        {
            return;
        }

        _configSub = _cfg.SubscribeMultiple()
            .OnValueChanged(ADTCCVars.DefaultGasPriceModifier, (value) => _defaultGasPriceModifier = value, true)
            .OnValueChanged(ADTCCVars.GasPriceModifierTritium, (value) => _gasPriceModifierTritium = value, true)
            .OnValueChanged(ADTCCVars.GasPriceModifierNitrousOxide, (value) => _gasPriceModifierNitrousOxide = value, true)
            .OnValueChanged(ADTCCVars.GasPriceModifierFrezon, (value) => _gasPriceModifierFrezon = value, true);
    }

    public float GetModifier(string id)
    {
        if (!Enum.TryParse<GasIds>(id, out var gasId))
            return _defaultGasPriceModifier;

        return gasId switch
        {
            GasIds.Tritium => _gasPriceModifierTritium,
            GasIds.NitrousOxide => _gasPriceModifierNitrousOxide,
            GasIds.Frezon => _gasPriceModifierFrezon,
            _ => _defaultGasPriceModifier,
        };
    }

    private void ShutdownADTAtmosCVars()
    {
        _configSub?.Dispose();
    }
}

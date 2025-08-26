using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.PlantAnalyzer;

/// <summary>
///     The information about the last scanned plant/seed is stored here.
/// </summary>
[Serializable, NetSerializable]
public sealed class PlantAnalyzerScannedSeedPlantInformation : BoundUserInterfaceMessage
{
    public NetEntity? TargetEntity;
    public bool IsTray;

    public string? SeedName;
    public string[]? SeedChem;
    public AnalyzerHarvestType HarvestType;
    public GasFlags ExudeGases;
    public GasFlags ConsumeGases;
    public float Endurance;
    public int SeedYield;
    public float Lifespan;
    public float Maturation;
    public float Production;
    public int GrowthStages;
    public float SeedPotency;
    public string[]? Speciation; // Currently only available on server, we need to send strings to the client.
    public AdvancedScanInfo? AdvancedInfo;
}

/// <summary>
///     Information gathered in an advanced scan.
/// </summary>
[Serializable, NetSerializable]
public struct AdvancedScanInfo
{
    public float NutrientConsumption;
    public float WaterConsumption;
    public float IdealHeat;
    public float HeatTolerance;
    public float IdealLight;
    public float LightTolerance;
    public float ToxinsTolerance;
    public float LowPressureTolerance;
    public float HighPressureTolerance;
    public float PestTolerance;
    public float WeedTolerance;
    public MutationFlags Mutations;
}

// Note: currently leaving out Viable.
[Flags]
public enum MutationFlags : byte
{
    None = 0,
    TurnIntoKudzu = 1,
    Seedless = 2,
    Slip = 4,
    Sentient = 8,
    Ligneous = 16,
    Bioluminescent = 32,
    CanScream = 64,
}

[Flags]
public enum GasFlags : int
{
    None = 0,
    Nitrogen = 1 << 0,
    Oxygen = 1 << 1,
    CarbonDioxide = 1 << 2,
    Plasma = 1 << 3,
    Tritium = 1 << 4,
    WaterVapor = 1 << 5,
    Ammonia = 1 << 6,
    NitrousOxide = 1 << 7,
    Frezon = 1 << 8,
    BZ = 1 << 9,
    Halon = 1 << 10,
    Healium = 1 << 11,
    HyperNoblium = 1 << 12,
    Hydrogen = 1 << 13,
    Pluoxium = 1 << 14,
    Nitrium = 1 << 15,
    Helium = 1 << 16,
    AntiNoblium = 1 << 17,
    ProtoNitrate = 1 << 18,
    Zauker = 1 << 19,
}

public enum AnalyzerHarvestType : byte
{
    Unknown, // Just in case the backing enum type changes and we haven't caught it.
    Repeat,
    NoRepeat,
    SelfHarvest
}


[Serializable, NetSerializable]
public sealed class PlantAnalyzerSetMode : BoundUserInterfaceMessage
{
    public bool AdvancedScan { get; }
    public PlantAnalyzerSetMode(bool advancedScan)
    {
        AdvancedScan = advancedScan;
    }
}

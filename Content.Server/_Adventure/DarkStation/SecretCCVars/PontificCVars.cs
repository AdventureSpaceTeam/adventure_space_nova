using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Server._Adventure.SecretCCVars;

[CVarDefs]
public sealed class PontificCVars : CVars
{
    public static readonly CVarDef<int> PontificFelProduceTime =
        CVarDef.Create("pontific_fel_radius_produce_time", 120);

    public static readonly CVarDef<float> PontificFelRadiusDamage =
        CVarDef.Create("pontific.fel_radius_damage", 10f);

    public static readonly CVarDef<int> PontificFelRadiusIncrease =
        CVarDef.Create("pontific.fel_radius_increase", 2);

    public static readonly CVarDef<int> PontificFelRadiusIncreaseTime =
        CVarDef.Create("pontific.fel_radius_increase_time", 120);

    public static readonly CVarDef<int> PontificFaithTime =
        CVarDef.Create("pontific.faith_time", 10);

    public static readonly CVarDef<int> PontificFlameTime =
        CVarDef.Create("pontific.flame_time", 20);
}

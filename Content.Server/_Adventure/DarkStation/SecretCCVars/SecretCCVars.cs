using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Server._Adventure.SecretCCVars;

[CVarDefs]
public sealed class SecretCCVars : CVars
{
    public static readonly CVarDef<bool> IsFakeNumbersEnabled =
        CVarDef.Create("config.is_fake_numbers_enabled", true, CVar.SERVERONLY);

    public static readonly CVarDef<int> NarsiMinPlayers =
        CVarDef.Create("narsi.min_players", 30, CVar.SERVERONLY);

    public static readonly CVarDef<int> VampireMinPlayers =
        CVarDef.Create("vampire.min_players", 30, CVar.SERVERONLY);

    public static readonly CVarDef<int> VampireBloodObjectiveMax =
        CVarDef.Create("vampire.blood_objective_max", 4000, CVar.SERVERONLY);

    public static readonly CVarDef<int> VampireBloodObjectiveMin =
        CVarDef.Create("vampire.blood_objective_min", 3000, CVar.SERVERONLY);

    public static readonly CVarDef<bool> IsSeveredLimbEnabled =
        CVarDef.Create("surgery.severed_limb_enabled", true);

    public static readonly CVarDef<int> RatvarMaxRighteousCount =
        CVarDef.Create("ratvar.max_righteous_count", 6);

    public static readonly CVarDef<bool> IsDeseasesEnabled =
        CVarDef.Create("diseases.enabled", false);

    public static readonly CVarDef<bool> IsHeartAttackEnabled =
        CVarDef.Create("surgery.heart_attack_enabled", false);

    public static readonly CVarDef<bool> IsHeartStrainEnabled =
        CVarDef.Create("surgery.heart_strain_enabled", false);
}

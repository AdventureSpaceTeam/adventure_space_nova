using Content.Shared.AdventureSpace.DarkForces.Ratvar.Righteous.Roles;

namespace Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Progress;

public sealed partial class RatvarProgressSystem
{
    // [ValidatePrototypeId<LanguagePrototype>]
    // private const string LanguagePrototype = "Ratvar";
    //
    // [Dependency] private readonly LanguageSystem _languageSystem = default!;

    public void SetupRighteous(EntityUid uid)
    {
        // _languageSystem.AddLanguage(uid, LanguagePrototype, true, true);
        if (_progressEntity?.Comp is not { } comp)
            return;

        AddObjectivesToRighteous(
            uid,
            comp.RatvarBeaconsObjective,
            comp.RatvarConvertObjective,
            comp.RatvarPowerObjective,
            comp.RatvarSummonObjective
        );
    }

    private bool CanUseRatvarItems(EntityUid uid)
    {
        return HasComp<RatvarRighteousComponent>(uid);
    }
}

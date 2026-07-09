using Content.Shared.RPSX.DarkForces.Ratvar.Righteous.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Progress;

public sealed partial class RatvarProgressSystem
{
    public void SetupRighteous(EntityUid uid)
    {
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

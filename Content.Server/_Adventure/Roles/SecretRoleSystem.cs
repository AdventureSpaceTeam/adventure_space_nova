using Content.Server.AdventurePrivate._Alteros.DarkForces.Narsi.Cultist.Roles;
using Content.Server.AdventurePrivate._Alteros.DarkForces.Ratvar.Righteous.Progress.Roles;
using Content.Server.AdventurePrivate._Alteros.GameRules.Vampire.Role.Components;
using Content.Server.Roles;
using VampireTrallRoleComponent =
    Content.Server.AdventurePrivate._Alteros.GameRules.Vampire.Role.Trall.VampireTrallRoleComponent;

namespace Content.Server.AdventurePrivate._Alteros.Roles;

public sealed class SecretRoleSystem : EntitySystem
{
    [Dependency] private readonly RoleSystem _roleSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        _roleSystem.SubscribeAntagEvents<NarsiCultRoleComponent>();
        _roleSystem.SubscribeAntagEvents<VampireRoleComponent>();
        _roleSystem.SubscribeAntagEvents<VampireTrallRoleComponent>();
        _roleSystem.SubscribeAntagEvents<RatvarRoleComponent>();
    }
}

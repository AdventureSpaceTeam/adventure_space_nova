using Content.Shared.RPSX;
using Content.Shared.RPSX.DarkForces.Narsi.Abilities.Events;
using Content.Shared.RPSX.DarkForces.Narsi.Cultist.Shadow;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Spawners;

namespace Content.Server.RPSX.DarkForces.Narsi.Cultist.Abilities;

public sealed partial class NarsiCultistAbilitiesSystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;

    private void InitializeShadow()
    {
        SubscribeLocalEvent<NarsiCultistComponent, NarsiCultistShadowEvent>(OnShadowEvent);
    }

    private void OnShadowEvent(EntityUid uid, NarsiCultistComponent component, NarsiCultistShadowEvent args)
    {
        if (args.Handled)
            return;

        var transform = Transform(uid);
        var level = _progressSystem.GetAbilityLevel(ShadowAction);
        var shadowsCount = level switch
        {
            1 => 1,
            2 => 2,
            _ => 3
        };

        SharedUtils.Repeat(shadowsCount, () =>
        {
            var shadow = Spawn("MobCultistShadow", transform.Coordinates);

            SetupTimedDespawn(shadow, level);
            SetupShadowComponent(uid, shadow, level);
            CopyData(uid, shadow);
        });
        OnCultistAbility(uid, args);
        args.Handled = true;
    }

    private void SetupTimedDespawn(EntityUid shadow, int level)
    {
        var timed = EnsureComp<TimedDespawnComponent>(shadow);
        timed.Lifetime = level switch
        {
            1 => 20,
            2 => 40,
            _ => 60
        };
    }

    private void SetupShadowComponent(EntityUid cultist, EntityUid shadow, int level)
    {
        var shadowComponent = new NarsiCultistShadowVisualizeComponent
        {
            Owner = shadow,
            EntityToCopy = GetNetEntity(cultist)
        };

        EntityManager.AddComponent(shadow, shadowComponent);
    }

    private void CopyData(EntityUid cultist, EntityUid shadow)
    {
        var cultistMeteData = MetaData(cultist);

        _metaData.SetEntityName(shadow, cultistMeteData.EntityName);
        _metaData.SetEntityDescription(shadow, cultistMeteData.EntityDescription);
    }
}

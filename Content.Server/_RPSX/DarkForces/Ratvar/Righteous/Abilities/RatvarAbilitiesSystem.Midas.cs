using System.Linq;
using Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities.Midas;
using Content.Server.Stack;
using Content.Shared.RPSX.DarkForces.Ratvar.Prototypes;
using Content.Shared.RPSX.DarkForces.Ratvar.Righteous.Abilities;
using Content.Shared.RPSX.DarkForces.Ratvar.UI;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using MidasMaterialComponent = Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities.Midas.MidasMaterialComponent;
using MidasTargetComponent = Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities.Midas.MidasTargetComponent;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Abilities;

public sealed partial class RatvarAbilitiesSystem
{
    [Dependency] private readonly StackSystem _stack = default!;

    private void InitMidasTouch()
    {
        SubscribeLocalEvent<RatvarAbilitiesComponent, RatvarMidasTouchEvent>(OnMidasTouch);
        SubscribeLocalEvent<MidasMaterialComponent, RatvarTouchSelectedMessage>(OnMidasSelected);
    }

    private void OnMidasSelected(EntityUid uid, MidasMaterialComponent component,
        RatvarTouchSelectedMessage args)
    {
        if (!_prototype.TryIndex<RatvarMidasTouchablePrototype>(args.Item, out var midasPrototype))
            return;

        SpawnItem(midasPrototype.Item, uid);
    }

    private void OnMidasTouch(EntityUid uid, RatvarAbilitiesComponent component, RatvarMidasTouchEvent args)
    {
        OnMidasTouch(uid, args);
    }

    private void OnMidasTouch(EntityUid uid, RatvarMidasTouchEvent args)
    {
        if (args.Handled)
            return;

        var target = args.Target;
        if (TryComp<MidasMaterialComponent>(target, out var material))
        {
            OnMidasTouchMaterial(uid, target, material);
            args.Handled = true;
            return;
        }

        if (HasComp<MidasTargetComponent>(target))
        {
            OnMidasTouchTarget(uid, target);
            args.Handled = true;
        }
    }

    private void OnMidasTouchTarget(EntityUid uid, EntityUid target)
    {
        var ev = new MidasTargetEvent(uid);
        RaiseLocalEvent(target, ref ev);
    }

    private void OnMidasTouchMaterial(EntityUid uid, EntityUid target, MidasMaterialComponent material)
    {
        if (material.Targets.Count == 1 && _prototype.TryIndex(material.Targets.First(), out var midasTouchable))
        {
            SpawnItem(midasTouchable.Item, target);
            return;
        }

        if (!_ui.HasUi(target, RatvarMidasTouchUIKey.Key))
            return;

        var ids = material.Targets.Select(ent => ent.Id).ToList();

        _ui.SetUiState(target, RatvarMidasTouchUIKey.Key, new RatvarMidasTouchBUIState(ids));
        _ui.OpenUi(target,  RatvarMidasTouchUIKey.Key, uid);
    }

    private void SpawnItem(string item, EntityUid target)
    {
        var transform = Transform(target);
        var spawnedItem = Spawn(item, transform.Coordinates);
        if (HasComp<StackComponent>(spawnedItem) && TryComp<StackComponent>(target, out var stackTarget))
            _stack.SetCount(spawnedItem, stackTarget.Count);

        QueueDel(target);
    }
}

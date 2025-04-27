using Content.Shared.AdventureSpace.Zombie.Smoker.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Client.AdventureSpace.Zombie;

public sealed class ZombieSmokerOverlay : Overlay
{
    private readonly IEntityManager _entManager;

    public ZombieSmokerOverlay(IEntityManager entManager)
    {
        _entManager = entManager;
    }

    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowFOV;

    protected override void Draw(in OverlayDrawArgs args)
    {
        var worldHandle = args.WorldHandle;

        var query = _entManager.EntityQueryEnumerator<ZombieSmokerComponent, TransformComponent>();
        var xformQuery = _entManager.GetEntityQuery<TransformComponent>();
        var targets = _entManager.GetEntityQuery<ZombieSmokerTargetComponent>();

        var xformSystem = _entManager.System<SharedTransformSystem>();
        var spriteSystem = _entManager.System<SpriteSystem>();

        while (query.MoveNext(out var uid, out var smoker, out var xForm))
        {
            var target = smoker.CurrentTarget;
            if (target == EntityUid.Invalid)
                continue;

            if (!targets.HasComponent(target))
                continue;

            if (!xformQuery.TryGetComponent(target, out var targetXForm))
                continue;

            if (xForm.MapID != targetXForm.MapID)
                continue;

            var texture = spriteSystem.Frame0(smoker.Tongue);
            var width = texture.Width / (float)EyeManager.PixelsPerMeter;

            var coordsA = xForm.Coordinates;
            var coordsB = targetXForm.Coordinates;

            var posA = coordsA.ToMapPos(_entManager, xformSystem);
            var posB = coordsB.ToMapPos(_entManager, xformSystem);
            var diff = posB - posA;
            var length = diff.Length();

            var midPoint = diff / 2f + posA;
            var angle = (posB - posA).ToWorldAngle();
            var box = new Box2(-width / 2f, -length / 2f, width / 2f, length / 2f);
            var rotate = new Box2Rotated(box.Translated(midPoint), angle, midPoint);

            worldHandle.DrawTextureRect(texture, rotate);
        }
    }
}

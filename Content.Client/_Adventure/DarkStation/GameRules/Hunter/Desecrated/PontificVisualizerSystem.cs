using Content.Client._Adventure.DarkForces.Desecrated;
using Content.Shared._Adventure.Hunter.Desecrated.Pontific;
using Robust.Client.GameObjects;

namespace Content.Client._Adventure.GameRules.Hunter.Desecrated;

public sealed class PontificDamageStateVisualizerSystem : VisualizerSystem<PontificVisualsComponent>
{
    protected override void OnAppearanceChange(
        EntityUid uid,
        PontificVisualsComponent component,
        ref AppearanceChangeEvent args
    )
    {
        var sprite = args.Sprite;
        if (sprite == null || !AppearanceSystem.TryGetData<PontificState>(uid,
                PontificStateVisuals.State,
                out var pontificState,
                args.Component))
            return;

        sprite.LayerSetVisible(PontificVisualLayers.Base, pontificState == PontificState.Base);
        sprite.LayerSetVisible(PontificVisualLayers.Dead, pontificState == PontificState.Dead);
        sprite.LayerSetVisible(PontificVisualLayers.Flame, pontificState == PontificState.Flame);
        // sprite.LayerSetVisible(PontificVisualLayers.Prayer, pontificState == PontificState.Prayer);
    }
}

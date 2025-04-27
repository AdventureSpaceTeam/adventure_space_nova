using Content.Shared._Adventure.Medical.Diseases.Effects;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._Adventure.Medical.Disease;

public sealed class DiseasesVisualizerSystem : VisualizerSystem<DiseasesVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid,
        DiseasesVisualsComponent component,
        ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        foreach (DiseaseVisualLayers layer in Enum.GetValues(typeof(DiseaseVisualLayers)))
        {
            if (!AppearanceSystem.TryGetData(uid, layer, out var data))
                continue;

            switch (data)
            {
                case DiseaseClearKey.Clear:
                    args.Sprite.LayerSetVisible(layer, false);
                    break;

                case DiseaseVisualsData visualsData:
                    var spriteSpecifier = new SpriteSpecifier.Rsi(new ResPath(visualsData.Sprite), visualsData.State);

                    args.Sprite.LayerSetSprite(layer, spriteSpecifier);
                    args.Sprite.LayerSetVisible(layer, true);

                    break;
            }
        }
    }
}

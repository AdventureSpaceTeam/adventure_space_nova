using Content.Shared._Adventure.EnergyCores;
using Robust.Client.GameObjects;
using Content.Client.Power;

namespace Content.Client._Adventure.EnergyCores;

public sealed partial class EnergyCoreSystem : VisualizerSystem<EnergyCoreVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, EnergyCoreVisualsComponent comp, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;
        if (AppearanceSystem.TryGetData<EnergyCoreState>(uid, EnergyCoreVisualLayers.IsOn, out var res, args.Component))
        {
            SpriteSystem.LayerSetVisible((uid, args.Sprite), PowerDeviceVisualLayers.Powered, false);
            foreach (var cur in args.Sprite.AllLayers)
            {
                cur.Visible = false;
            }
            switch (res)
            {
                case EnergyCoreState.Enabled:
                    SpriteSystem.LayerSetVisible((uid, args.Sprite), EnergyCoreVisualLayers.IsOn, true);
                    break;
                case EnergyCoreState.Disabled:
                    SpriteSystem.LayerSetVisible((uid, args.Sprite), EnergyCoreVisualLayers.IsOff, true);
                    break;
                case EnergyCoreState.Enabling:
                    SpriteSystem.LayerSetVisible((uid, args.Sprite), EnergyCoreVisualLayers.Enabling, true);
                    break;
                case EnergyCoreState.Disabling:
                    SpriteSystem.LayerSetVisible((uid, args.Sprite), EnergyCoreVisualLayers.Disabling, true);
                    break;
                default: Log.Error("Incorrect state by " + uid); break;
            }
        }
    }
}

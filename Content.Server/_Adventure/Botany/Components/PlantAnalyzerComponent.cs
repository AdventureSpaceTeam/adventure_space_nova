using Content.Shared.DoAfter;
using Robust.Shared.Audio;

namespace Content.Server.Botany.Components;

/// <summary>
///    After scanning, retrieves the target Uid to use with its related UI.
/// </summary>
[RegisterComponent]
public sealed partial class PlantAnalyzerComponent : Component
{
    [DataDefinition]
    public partial struct PlantAnalyzerSetting
    {
        [DataField]
        public bool AdvancedScan;

        [DataField]
        public float ScanDelay;

        [DataField]
        public float AdvScanDelay;
    }

    [DataField, ViewVariables]
    public PlantAnalyzerSetting Settings = new()
    {
        AdvancedScan = false,
        ScanDelay = 1.0f,
        AdvScanDelay = 5.0f
    };

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public DoAfterId? DoAfter;

    [DataField]
    public SoundSpecifier ScanningEndSound = new SoundPathSpecifier("/Audio/Items/Medical/healthscanner.ogg");
}

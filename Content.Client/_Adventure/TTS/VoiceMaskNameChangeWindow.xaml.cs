using System.Linq;
using Content.Shared._Adventure.ACVar;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Content.Shared._Adventure.TTS;
using Content.Client.UserInterface.Controls;

namespace Content.Client.VoiceMask;

public sealed partial class VoiceMaskNameChangeWindow : FancyWindow
{
    public Action<string>? OnVoiceChange;
    private List<TTSVoicePrototype> _voices = new();
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private void ReloadVoices()
    {
        if (_cfg is null)
            return;
        TTSContainer.Visible = _cfg.GetCVar(ACVars.TTSEnabled);
        if (!_cfg.GetCVar(ACVars.TTSEnabled))
            return;
        VoiceSelector.OnItemSelected += args =>
        {
            VoiceSelector.SelectId(args.Id);
            if (VoiceSelector.SelectedMetadata != null)
                OnVoiceChange?.Invoke((string)VoiceSelector.SelectedMetadata);
        };
        _voices = _proto
            .EnumeratePrototypes<TTSVoicePrototype>()
            .OrderBy(o => Loc.GetString(o.Name))
            .OrderBy(o => ((o.Gender == "male") ? 0b01 : 0) + ((o.Gender == "female") ? 0b10 : 0))
            .ToList();
        for (var i = 0; i < _voices.Count; i++)
        {
            var name = Loc.GetString(_voices[i].Name);
            VoiceSelector.AddItem(name);
            VoiceSelector.SetItemMetadata(i, _voices[i].ID);
        }
    }
}

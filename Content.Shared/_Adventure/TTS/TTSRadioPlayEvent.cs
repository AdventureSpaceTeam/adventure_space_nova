using Content.Shared.Speech;
using Robust.Shared.Prototypes;
using Content.Shared.Inventory;

namespace Content.Shared._Adventure.TTS;

public sealed class TTSRadioPlayEvent : EntityEventArgs
{
    public string Message;
    public string Voice;

    public TTSRadioPlayEvent(string message, string voice)
    {
        Message = message;
        Voice = voice;
    }
}

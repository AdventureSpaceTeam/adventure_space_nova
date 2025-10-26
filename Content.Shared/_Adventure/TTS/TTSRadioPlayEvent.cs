using Content.Shared.Speech;
using Robust.Shared.Prototypes;
using Content.Shared.Inventory;

namespace Content.Shared._Adventure.TTS;

public sealed class TTSRadioPlayEvent : EntityEventArgs
{
    public string Message;
    public string Voice;
    public NetEntity? Source;
    public NetEntity? Author;

    public TTSRadioPlayEvent(string message, string voice, NetEntity? source, NetEntity? author)
    {
        Message = message;
        Voice = voice;
        Source = source;
        Author = author;
    }
}

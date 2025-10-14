using Robust.Shared.Serialization;

namespace Content.Shared._Adventure.TTS;

[Serializable, NetSerializable]
// ReSharper disable once InconsistentNaming
public sealed class PlayTTSEvent : EntityEventArgs
{
    public byte[] Data { get; }
    /// <summary>
    /// Source of the sound.
    /// </summary>
    public NetEntity? SourceUid { get; }
    /// <summary>
    /// Author of the sound.
    /// Used for audio queue.
    /// </summary>
    public NetEntity? Author { get; }
    public bool IsWhisper { get; }

    public PlayTTSEvent(byte[] data, NetEntity? sourceUid = null, bool isWhisper = false, NetEntity? author = null)
    {
        Data = data;
        SourceUid = sourceUid;
        IsWhisper = isWhisper;
        Author = author ?? sourceUid;
    }
}

using Content.Shared.Chat;
using Content.Shared._Adventure.ACVar;
using Content.Shared._Adventure.TTS;
using Robust.Client.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Utility;
using System.IO;

namespace Content.Client._Adventure.TTS;

internal record struct TTSQueueElem(AudioStream Audio, bool IsWhisper);

/// <summary>
/// Plays TTS audio in world
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class TTSSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IAudioManager _audioLoader = default!;

    private ISawmill _sawmill = default!;
    private bool _enabled = true;

    /// <summary>
    /// Reducing the volume of the TTS when whispering. Will be converted to logarithm.
    /// </summary>
    private const float WhisperFade = 4f;

    /// <summary>
    /// The volume at which the TTS sound will not be heard.
    /// </summary>
    private const float MinimalVolume = -10f;

    /// <summary>
    /// Maximum queued tts talks per entity.
    /// </summary>
    private const int MaxQueuedSounds = 20;

    private float _volume = 0.0f;

    private Dictionary<NetEntity, Queue<TTSQueueElem>> _queue = new();
    private Dictionary<EntityUid, AudioComponent?> _playing = new();

    public override void Initialize()
    {
        _sawmill = Logger.GetSawmill("tts");
        _cfg.OnValueChanged(ACVars.TTSVolume, OnTtsVolumeChanged, true);
        _cfg.OnValueChanged(ACVars.TTSClientEnabled, OnTtsClientOptionChanged, true);
        SubscribeNetworkEvent<PlayTTSEvent>(OnPlayTTS);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _cfg.UnsubValueChanged(ACVars.TTSVolume, OnTtsVolumeChanged);
        _cfg.UnsubValueChanged(ACVars.TTSClientEnabled, OnTtsClientOptionChanged);
    }

    public override void FrameUpdate(float frameTime)
    {
        if (!_enabled) return;
        foreach (var (uid, comp) in _playing)
        {
            _sawmill.Debug($"Iterating _playing: {uid} -> {comp}");
            if (comp is not null && !comp.Playing)
            {
                _sawmill.Error($"Removing audio component for entity {uid}");
                _playing[uid] = null;
            }
        }

        foreach (var (speaker, queue) in _queue)
        {
            _sawmill.Debug($"Iterating _queue: {speaker}");
            if (queue.Count <= 0) continue;
            _sawmill.Debug($"Queue not empty, continuing");
            EntityUid local_speaker = EntityUid.Invalid;
            EntityUid? source = null;
            if (speaker.Valid)
            {
                if (!TryGetEntity(speaker, out var uid) || uid is null)
                {
                    _sawmill.Error($"Couldn't get entity {speaker}, clearing queue");
                    queue.Clear();
                    continue;
                }
                local_speaker = uid.Value;
                source = uid.Value;
            }
            if (!(_playing.ContainsKey(local_speaker) && _playing[local_speaker] is null)) continue;
            if (!queue.TryDequeue(out var elem)) continue;
            _sawmill.Error($"Dequeued tts speak for {speaker}");

            _playing[local_speaker] = PlayTTSFromUid(source, elem.Audio, elem.IsWhisper);
        }
    }

    public AudioComponent? PlayTTSFromUid(EntityUid? uid, AudioStream audioStream, bool isWhisper)
    {
        var audioParams = AudioParams.Default
            .WithVolume(AdjustVolume(isWhisper))
            .WithMaxDistance(AdjustDistance(isWhisper));
        (EntityUid Entity, AudioComponent Component)? stream;

        _sawmill.Error($"Playing TTS audio {audioStream.Length} bytes from {uid} entity");

        if (uid is not null)
        {
            stream =  _audio.PlayEntity(audioStream, uid.Value, null, audioParams);
        }
        else
        {
            stream = _audio.PlayGlobal(audioStream, null, audioParams);
        }

        _sawmill.Error($"Resulting stream: {stream}");

        return stream?.Component;
    }

    public void RequestPreviewTTS(string voiceId)
    {
        RaiseNetworkEvent(new RequestPreviewTTSEvent(voiceId));
    }

    private void OnTtsClientOptionChanged(bool option)
    {
        _enabled = option;
        RaiseNetworkEvent(new ClientOptionTTSEvent(option));
    }

    private void OnTtsVolumeChanged(float volume)
    {
        _volume = volume;
    }

    private void OnPlayTTS(PlayTTSEvent ev)
    {
        var source = ev.SourceUid ?? NetEntity.Invalid;
        if (!_queue.ContainsKey(source))
            _queue[source] = new();

        if (_queue[source].Count >= MaxQueuedSounds)
            return;

        var audioStream = _audioLoader.LoadAudioWav(new MemoryStream(ev.Data));

        _sawmill.Error($"Enqueuing audio for entity |{source}|");
        _queue[source].Enqueue(new TTSQueueElem
        {
            Audio = audioStream,
            IsWhisper = ev.IsWhisper,
        });

        if (!(ev.SourceUid?.Valid ?? false))
        {
            _sawmill.Error($"Adding empty value for entity |{EntityUid.Invalid}|");
            _playing.TryAdd(EntityUid.Invalid, null);
        }
        else if (TryGetEntity(ev.SourceUid, out var ent) && ent is not null)
        {
            _sawmill.Error($"Adding empty value for entity |{ent}|");
            _playing.TryAdd(ent.Value, null);
        }
        else
        {
            _sawmill.Error($"SourceUid can't be retrieved");
        }
    }

    private float AdjustVolume(bool isWhisper)
    {
        var volume = MinimalVolume + SharedAudioSystem.GainToVolume(_volume * 3.0f);

        if (isWhisper)
        {
            volume -= SharedAudioSystem.GainToVolume(WhisperFade);
        }

        return volume;
    }

    private float AdjustDistance(bool isWhisper)
    {
        return isWhisper ? TTSConfig.WhisperMuffledRange : TTSConfig.VoiceRange;
    }
}

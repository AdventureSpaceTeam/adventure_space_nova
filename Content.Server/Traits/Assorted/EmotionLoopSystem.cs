using Robust.Shared.Random;
using Content.Server.Chat.Systems;

namespace Content.Server.Traits.Assorted;

/// <summary>
/// Эта система позволяет вызывать любую эмоцию через случайные промежутки времени.
/// </summary>
public sealed class EmotionLoopSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<EmotionLoopComponent, ComponentStartup>(SetupTimer);
    }

    private void SetupTimer(EntityUid uid, EmotionLoopComponent component, ComponentStartup args)
    {
        component.NextIncidentTime =
            _random.NextFloat(component.TimeBetweenEmotions.X, component.TimeBetweenEmotions.Y);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<EmotionLoopComponent>();
        while (query.MoveNext(out var uid, out var emotionLoop))
        {
            emotionLoop.NextIncidentTime -= frameTime;

            if (emotionLoop.NextIncidentTime >= 0)
                continue;

            // Устанавливаем обновленное время.
            emotionLoop.NextIncidentTime +=
                _random.NextFloat(emotionLoop.TimeBetweenEmotions.X, emotionLoop.TimeBetweenEmotions.Y);

            // Проигрываем эмоцию, указанную в компоненте.
            _chat.TryEmoteWithChat(uid, emotionLoop.Emotion, ignoreActionBlocker: false);
        }
    }
}

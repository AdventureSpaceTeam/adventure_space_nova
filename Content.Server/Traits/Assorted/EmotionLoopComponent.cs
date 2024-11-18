using System.Numerics;

namespace Content.Server.Traits.Assorted;

/// <summary>
/// Этот компонент позволяет вызывать любую эмоцию через случайные промежутки времени.
/// </summary>
[RegisterComponent, Access(typeof(EmotionLoopSystem))]
public sealed partial class EmotionLoopComponent : Component
{
    /// <summary>
    /// Случайный интервал между эмоциями (минимум, максимум).
    /// </summary>
    [DataField("timeBetweenEmotions", required: true)]
    public Vector2 TimeBetweenEmotions { get; private set; }

    [DataField(required: true)]
    public string Emotion { get; private set; }

    public float NextIncidentTime;
}

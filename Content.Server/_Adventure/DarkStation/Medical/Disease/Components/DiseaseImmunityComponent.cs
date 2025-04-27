namespace Content.Server._Adventure.Medical.Disease.Components;

/**
 * Полный иммунитет к болезни можно получить лишь в процессе болезни или раундстартом
 * Например прописать его расе
 * Иммунитет постепенно нарастает, опираясь на параметр ImmunityIncreaseType
 * Этот параметр отвечает за то, каким образом иммунитет накапливается
 * На текущий момент завозим два параметра Time (со временем, например: простуда сама может пройти и ты получишь временный иммунитет)
 * И Cure (принимать лекарство, например: грипп не пройдет сам, и нужно принимать лекарство, но ты получишь временный иммунитет к нему)
 */
[RegisterComponent]
public sealed partial class DiseaseImmunityComponent : Component
{
    [DataField]
    public float BaseImmunityFriction = 0.5f;

    [DataField]
    public float BaseImmunityPenalties = 10f;

    /**
     * Базовые показатели иммунитета, они будут отвечать за то, заразится и заболет ли человек той или иной болезнью
     * Некоторые болезни будут игнорировать эти параметры, имея 100% шанс на заражение
     * Но банальная простуда может не наступить, если человек регулярно улучшает свой иммунитет
     * В тоже время, если человек все же заболел, мы уменьшаем ему иммунитет, т.к. организм ослабел
     */
    [DataField]
    public float BaseMaxImmunity = 100f;

    [DataField]
    public Dictionary<string, DiseaseImmunityProgress> ImmunityDiseasesProgress = new();

    [DataField]
    public float ImmunityLevel = 25f;

    [DataField]
    public TimeSpan ImmunityPenaltyPeriod;

    [DataField]
    public TimeSpan ImmunityPenaltyThreshold = TimeSpan.FromMinutes(5);
}

[DataDefinition]
public sealed partial class DiseaseImmunityProgress
{
    [DataField]
    public bool CanDecrease;

    [DataField]
    public float Immunity;

    [DataField]
    public TimeSpan ImmunityDecreaseThreshold = TimeSpan.FromSeconds(30);

    [DataField]
    public TimeSpan ImmunityDecreaseTime;
}

[Serializable]
public enum ImmunityIncreaseType
{
    None,
    Cure,
    Time,
}

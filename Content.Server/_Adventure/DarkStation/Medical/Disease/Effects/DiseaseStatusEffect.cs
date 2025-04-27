using Content.Shared.StatusEffect;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.AdventureSpace.Medical.Disease.Effects;

[DataDefinition]
public sealed partial class DiseaseStatusEffect : DiseaseEffect
{
    public enum StatusEffectDiseaseType
    {
        Add,
        Remove,
        Set,
    }

    [DataField(required: true)]
    public string Component = default!;

    [DataField(required: true)]
    public string Key = default!;

    [DataField]
    public bool Refresh;

    [DataField(required: true, customTypeSerializer: typeof(TimespanSerializer))]
    public TimeSpan Time;

    [DataField]
    public StatusEffectDiseaseType Type = StatusEffectDiseaseType.Add;

    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);

        var statusSys = entityManager.EntitySysManager.GetEntitySystem<StatusEffectsSystem>();
        switch (Type)
        {
            case StatusEffectDiseaseType.Add when Component != string.Empty:
                statusSys.TryAddStatusEffect(target, Key, Time, Refresh, Component);
                break;
            case StatusEffectDiseaseType.Remove:
                statusSys.TryRemoveTime(target, Key, Time);
                break;
            case StatusEffectDiseaseType.Set:
                statusSys.TrySetTime(target, Key, Time);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

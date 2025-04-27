using Content.Server._Adventure.DarkForces.Narsi.Buildings.Altar.Rituals.Prototypes;
using Content.Shared._Adventure.DarkForces.Narsi.Buildings.Altar.Rituals;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Adventure.DarkForces.Narsi.Buildings.Altar;

[RegisterComponent]
public sealed partial class NarsiAltarComponent : Component
{
    [DataField]
    public EntityUid? ActiveSound;

    [DataField]
    public EntityUid? BuckledEntity;

    [DataField]
    public DoAfterId? DoAfterId;

    [DataField]
    public TimeSpan RitualsThreshold = TimeSpan.FromSeconds(120);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan RitualTimeoutTick;

    [DataField]
    public NarsiRitualsProgressState State = NarsiRitualsProgressState.Idle;

    [DataField(required: true)]
    public NarsiRitualVisualsParams VisualsParams = default!;

    public NarsiRitualPrototype? ActualRitual { get; set; }
}

[DataDefinition]
public sealed partial class NarsiRitualVisualsParams
{
    [DataField]
    public List<EntProtoId> VisualsEntities = new();

    [DataField]
    public TimeSpan VisualsThreshold = TimeSpan.FromSeconds(7);

    [DataField]
    public TimeSpan VisualsTick = TimeSpan.Zero;
}

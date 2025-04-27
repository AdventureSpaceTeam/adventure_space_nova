using Content.Shared.Body.Organ;

namespace Content.Server._Adventure.Medical.Surgery.Organs.Components.Base;

[ImplicitDataDefinitionForInheritors]
public abstract partial class BaseOrganComponent : Component
{
    [DataField]
    public OrganCondition Condition = OrganCondition.Good;

    [DataField]
    public float CriticalThreshold = 80f;

    [DataField]
    public float Damage;

    [DataField]
    public bool Embedded;

    [DataField]
    public float MaxDamageThreshold = 100f;

    [DataField(required: true)]
    public string OrganKey;

    [DataField]
    public float PotencyDamageThreshold = -1f;

    [DataField]
    public float WarningThreshold = 50f;

    [DataField]
    public bool Working = true;
}

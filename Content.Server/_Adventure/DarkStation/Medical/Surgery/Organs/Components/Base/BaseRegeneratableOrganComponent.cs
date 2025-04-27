namespace Content.Server.AdventureSpace.Medical.Surgery.Organs.Components.Base;

[ImplicitDataDefinitionForInheritors]
public abstract partial class BaseRegeneratableOrganComponent : BaseOrganComponent
{
    [DataField]
    [ViewVariables]
    public float RegenerationAmount = 10f;
}

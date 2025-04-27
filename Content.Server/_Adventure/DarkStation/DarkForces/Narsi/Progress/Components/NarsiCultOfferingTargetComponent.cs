namespace Content.Server.AdventureSpace.DarkForces.Narsi.Progress.Components;

[RegisterComponent]
public sealed partial class NarsiCultOfferingTargetComponent : Component
{
    [DataField]
    public List<EntityUid> Objectives = new();
}

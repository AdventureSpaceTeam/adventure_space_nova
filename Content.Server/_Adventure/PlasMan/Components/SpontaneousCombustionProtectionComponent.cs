using Content.Server._Adventure.PlasMan.EntitySystems;

namespace Content.Server._Adventure.PlasMan.Components;

[RegisterComponent]
[Access(typeof(SpontaneousCombustionSystem))]
public sealed partial class SpontaneousCombustionProtectionComponent : Component
{
    /// <summary>
    /// </summary>
    [DataField]
    public int ProtectionPercent = 75;

    public record struct GetSpontaneousCombustionProtectionValuesEvent
    {
        public int ProtectionPercent;
    }
}

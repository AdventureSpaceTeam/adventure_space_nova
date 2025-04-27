using Content.Server.AdventureSpace.Medical.Disease.Data;

namespace Content.Server.AdventureSpace.Medical.Disease.Components;

/**
 * Распространение болезни через жидкости на земле
 */
[RegisterComponent]
public sealed partial class DiseasePuddleSpreaderComponent : Component
{
    [DataField]
    public List<DiseaseData> Diseases { get; set; } = new();
}

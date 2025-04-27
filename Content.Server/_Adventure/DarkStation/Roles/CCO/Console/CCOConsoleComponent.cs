using Content.Shared._Adventure.Roles.CCO;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Prototypes;
using Color = Robust.Shared.Maths.Color;

namespace Content.Server._Adventure.Roles.CCO.Console;

[RegisterComponent]
public sealed partial class CcoConsoleComponent : Component
{
    [ViewVariables]
    [DataField]
    public Color AnnouncementColor = Color.YellowGreen;

    [ViewVariables]
    [DataField(required: true)]
    public string AnnouncementDisplayName = "ОЦК";

    [DataField]
    public List<Entity<CCOConsoleTargetComponent>> AvailableStations = [];

    [DataField]
    public EmergencyShuttleState EmergencyShuttleState;

    [DataField]
    public EntityUid? Station;

    [DataField]
    public ProtoId<NpcFactionPrototype> TargetFaction = "NanoTrasen";
}

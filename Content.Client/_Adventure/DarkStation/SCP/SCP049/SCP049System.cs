using Content.Shared.AdventureSpace.SCP.SCP_049;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;
using SCP049Component = Content.Shared.AdventureSpace.SCP.SCP_049.Components.SCP049Component;
using SCP049ThrallComponent = Content.Shared.AdventureSpace.SCP.SCP_049.Components.SCP049ThrallComponent;

namespace Content.Client.AdventureSpace.SCP.SCP049;

public sealed class SCP049System : SharedSCP049System
{
    [ValidatePrototypeId<StatusIconPrototype>]
    private const string SCPPlagueZombie = "SCPPlagueZombie";

    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCP049Component, GetStatusIconsEvent>(GetStatusIcon);
        SubscribeLocalEvent<SCP049ThrallComponent, GetStatusIconsEvent>(GetThrallStatusIcon);
    }

    private void GetStatusIcon(Entity<SCP049Component> ent, ref GetStatusIconsEvent args)
    {
        var icon = _prototype.Index<StatusIconPrototype>(SCPPlagueZombie);
        args.StatusIcons.Add(icon);
    }

    private void GetThrallStatusIcon(Entity<SCP049ThrallComponent> ent, ref GetStatusIconsEvent args)
    {
        var icon = _prototype.Index<StatusIconPrototype>(SCPPlagueZombie);
        args.StatusIcons.Add(icon);
    }
}

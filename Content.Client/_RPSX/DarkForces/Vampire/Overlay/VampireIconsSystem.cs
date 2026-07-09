using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.RPSX.DarkForces.Vampire.Overlay;

public sealed class VampireIconsSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VampireComponent, GetStatusIconsEvent>(OnGetStatusIcon);
        SubscribeLocalEvent<VampireTrallComponent, GetStatusIconsEvent>(OnGetTrallStatusIcon);
    }

    private void OnGetTrallStatusIcon(Entity<VampireTrallComponent> ent, ref GetStatusIconsEvent args)
    {
        var icon = _prototype.Index(ent.Comp.StatusIcon);
        args.StatusIcons.Add(icon);
    }

    private void OnGetStatusIcon(Entity<VampireComponent> ent, ref GetStatusIconsEvent args)
    {
        var icon = _prototype.Index(ent.Comp.StatusIcon);
        args.StatusIcons.Add(icon);
    }
}

using Content.Shared.GameTicking;
using Content.Shared.RPSX.DarkForces.Narsi.Buildings.Altar;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.RPSX.DarkForces.Narsi.Overlay;

public sealed class NarsiCultistIconsSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    [ValidatePrototypeId<FactionIconPrototype>]
    private const string NarsiCultistLeaderIcon = "NarsiCultistLeaderIcon";

    [ValidatePrototypeId<FactionIconPrototype>]
    private const string NarsiCultistIcon = "NarsiCultistIcon";


    private bool IsIconsRitualFinished;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NarsiCultistComponent, GetStatusIconsEvent>(OnGetStatusIcon);
        SubscribeLocalEvent<NarsiCultistLeaderComponent, GetStatusIconsEvent>(OnLeaderGetStatusIcon);
        SubscribeLocalEvent<NarsiCultCreatureComponent, GetStatusIconsEvent>(OnGetCreatureStatusIcon);

        SubscribeNetworkEvent<RoundRestartCleanupEvent>(RoundRestartCleanup);
        SubscribeNetworkEvent<NarsiIconsRitualFinishedEvent>(OnIconsRitualFinished);
    }

    private void RoundRestartCleanup(RoundRestartCleanupEvent ev)
    {
        IsIconsRitualFinished = false;
    }

    private void OnIconsRitualFinished(NarsiIconsRitualFinishedEvent ev)
    {
        IsIconsRitualFinished = true;
    }

    private void OnLeaderGetStatusIcon(Entity<NarsiCultistLeaderComponent> ent, ref GetStatusIconsEvent args)
    {
        if (!IsIconsRitualFinished)
            return;

        args.StatusIcons.Add(_prototype.Index<FactionIconPrototype>(NarsiCultistLeaderIcon));
    }
    private void OnGetCreatureStatusIcon(Entity<NarsiCultCreatureComponent> ent, ref GetStatusIconsEvent args)
    {
        if (!IsIconsRitualFinished)
            return;

        args.StatusIcons.Add(_prototype.Index<FactionIconPrototype>(NarsiCultistIcon));
    }

    private void OnGetStatusIcon(Entity<NarsiCultistComponent> ent, ref GetStatusIconsEvent args)
    {
        if (!IsIconsRitualFinished)
            return;

        if (HasComp<NarsiCultistLeaderComponent>(ent))
            return;

        args.StatusIcons.Add(_prototype.Index<FactionIconPrototype>(NarsiCultistIcon));
    }
}

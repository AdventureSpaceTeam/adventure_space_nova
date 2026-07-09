using System;
using Content.Server.RPSX.DarkForces.Saint.Chaplain.Components;
using Content.Shared.RPSX.DarkForces.Saint.Reagent.Events;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Shared.Humanoid;
using Content.Shared.Mind;
using Content.Shared.Mindshield.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NPC.Systems;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Content.Shared.Stunnable;
using Robust.Server.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.RPSX.DarkForces.Vampire.Role.Trall;

public sealed class VampireTrallSystem : EntitySystem
{
    [Dependency] private readonly NpcFactionSystem _factionSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly SharedRoleSystem _roleSystem = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;

    private static TimeSpan ParalyzeOnDeTrall = TimeSpan.FromSeconds(5);

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireTrallComponent, ComponentInit>(OnVampireTrallInit);
        SubscribeLocalEvent<VampireTrallComponent, ComponentShutdown>(OnVampireTrallShutdown);

        SubscribeLocalEvent<VampireTrallComponent, OnSaintWaterDrinkEvent>(OnVampireTrallDrinkSaintWater);
        SubscribeLocalEvent<VampireTrallComponent, OnSaintWaterFlammableEvent>(OnVampireTrallFlammableSaintWater);
        SubscribeLocalEvent<VampireTrallRoleComponent, GetBriefingEvent>(OnBriefing);
    }

    private void OnBriefing(EntityUid uid, VampireTrallRoleComponent component, ref GetBriefingEvent args)
    {
        if (!TryComp<MindComponent>(uid, out var mind) || mind.OwnedEntity == null)
            return;

        if (!TryComp<VampireTrallComponent>(mind.OwnedEntity, out var trallComponent))
            return;

        if (trallComponent.OwnerUid == EntityUid.Invalid)
            return;

        var ownerName = MetaData(trallComponent.OwnerUid).EntityName;

        args.Briefing = string.Empty;
        args.Append(Loc.GetString("vampire-trall-briefing", ("ownerName", ownerName)));
    }

    private void OnVampireTrallDrinkSaintWater(EntityUid uid, VampireTrallComponent component,
        OnSaintWaterDrinkEvent args)
    {
        if (args.Cancelled)
            return;

        RemComp<VampireTrallComponent>(uid);
        args.Cancel();
    }

    private void OnVampireTrallFlammableSaintWater(EntityUid uid, VampireTrallComponent component,
        OnSaintWaterFlammableEvent args)
    {
        if (args.Cancelled)
            return;

        RemComp<VampireTrallComponent>(uid);
        args.Cancel();
    }

    private void OnVampireTrallInit(EntityUid uid, VampireTrallComponent component, ComponentInit args)
    {
        if (!_mindSystem.TryGetMind(uid, out var mindId, out _))
            return;

        SetupAntagonistRole(uid, mindId, component);
    }

    private void SetupAntagonistRole(EntityUid uid, EntityUid mindId, VampireTrallComponent component)
    {
        var antagPrototype = _prototypeManager.Index<AntagPrototype>("Vampire");

        _roleSystem.MindRemoveRole<VampireTrallRoleComponent>(mindId);
        _roleSystem.MindAddRole(mindId, "VampireTrall");
        EnsureComp<VampireTrallRoleComponent>(mindId);

        _factionSystem.RemoveFaction(uid, "NanoTrasen", false);
        _factionSystem.AddFaction(uid, "Vampire");

        _popupSystem.PopupEntity(Loc.GetString("vampire-trall"), uid, uid);
        _roleSystem.MindPlaySound(mindId, component.Alert);
    }

    private void RemoveAntagonistRole(EntityUid uid, EntityUid mindId)
    {
        _roleSystem.MindRemoveRole<VampireTrallRoleComponent>(mindId);

        _factionSystem.RemoveFaction(uid, "Vampire");
        _factionSystem.AddFaction(uid, "NanoTrasen");

        _popupSystem.PopupEntity(Loc.GetString("vampire-trall-free"), uid, uid);

        _stun.TryAddParalyzeDuration(uid, ParalyzeOnDeTrall);
    }

    private void OnVampireTrallShutdown(EntityUid uid, VampireTrallComponent component, ComponentShutdown args)
    {
        if (!_mindSystem.TryGetMind(uid, out var mindId, out _))
            return;

        RemoveAntagonistRole(uid, mindId);
    }

    public bool CanBeTrall(EntityUid target)
    {
        if (!HasComp<HumanoidAppearanceComponent>(target) || HasComp<VampireTrallComponent>(target))
            return false;

        if (HasComp<ChaplainComponent>(target) || HasComp<MindShieldComponent>(target))
            return false;

        return _mobStateSystem.IsAlive(target);
    }

    public void MakeTrall(EntityUid owner, EntityUid target)
    {
        var trallComponent = EnsureComp<VampireTrallComponent>(target);
        trallComponent.OwnerUid = owner;
    }
}

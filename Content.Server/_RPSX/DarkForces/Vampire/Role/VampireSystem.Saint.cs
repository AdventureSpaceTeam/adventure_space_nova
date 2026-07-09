using Content.Server.RPSX.DarkForces.Saint.Saintable.Events;
using Content.Server.RPSX.DarkForces.Vampire.Role.Components;
using Content.Shared.RPSX.DarkForces.Saint.Reagent.Events;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.RPSX.DarkForces.Vampire.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Content.Shared.Atmos.Components;

namespace Content.Server.RPSX.DarkForces.Vampire.Role;

public sealed partial class VampireSystem
{
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly FlammableSystem _flammableSystem = default!;

    private void InitializeSaint()
    {
        SubscribeLocalEvent<VampireComponent, OnSaintWaterDrinkEvent>(OnVampireDrinkSaintWater);
        SubscribeLocalEvent<VampireComponent, OnSaintWaterFlammableEvent>(OnVampireFlammableSaintWater);

        SubscribeLocalEvent<VampireComponent, OnSaintEntityAfterInteract>(OnVampireSaintOrSilverEntity);
        SubscribeLocalEvent<VampireComponent, OnSaintEntityCollide>(OnVampireSaintOrSilverEntity);
        SubscribeLocalEvent<VampireComponent, OnSaintEntityTryPickedUp>(OnVampireSaintOrSilverEntity);
        SubscribeLocalEvent<VampireComponent, OnSaintEntityHandInteract>(OnVampireSaintOrSilverEntity);

        SubscribeLocalEvent<VampireComponent, OnSilverEntityAfterInteract>(OnVampireSaintOrSilverEntity);
        SubscribeLocalEvent<VampireComponent, OnSilverEntityCollide>(OnVampireSaintOrSilverEntity);
        SubscribeLocalEvent<VampireComponent, OnSilverEntityHandInteract>(OnVampireSaintOrSilverEntity);
        SubscribeLocalEvent<VampireComponent, OnSilverEntityTryPickedUp>(OnVampireSaintOrSilverEntity);
        SubscribeLocalEvent<VampireComponent, OnTryPryingSilverEvent>(OnVampireSaintOrSilverEntity);
        SubscribeLocalEvent<VampireComponent, OnTryPryingSaintedEvent>(OnVampireSaintOrSilverEntity);
    }

    private void OnVampireSaintOrSilverEntity(EntityUid uid, VampireComponent component, ISaintEntityEvent args)
    {
        args.PushOnCollide = false;
        args.IsHandled = true;

        _popupSystem.PopupEntity(Loc.GetString("vampire-got-saint-damage"), uid, uid);
    }

    private void OnVampireFlammableSaintWater(EntityUid uid, VampireComponent component,
        OnSaintWaterFlammableEvent args)
    {
        if (args.Cancelled)
            return;

        IgniteVampire(uid, component);
        args.Cancel();
    }

    private void OnVampireDrinkSaintWater(EntityUid uid, VampireComponent component, OnSaintWaterDrinkEvent args)
    {
        if (args.Cancelled)
            return;

        IgniteVampire(uid, component);
        args.Cancel();
    }

    private void IgniteVampire(EntityUid uid, VampireComponent component)
    {
        if (!TryComp<FlammableComponent>(uid, out var flammableComponent) || component.FullPower)
            return;

        flammableComponent.FireStacks = 2;
        _flammableSystem.Ignite(uid, uid);
    }
}

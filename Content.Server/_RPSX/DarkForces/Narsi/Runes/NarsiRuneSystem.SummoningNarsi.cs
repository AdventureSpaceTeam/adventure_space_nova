using System;
using System.Linq;
using Content.Server.RPSX.DarkForces.Narsi.Cultist.Roles.Narsi;
using Content.Server.RPSX.DarkForces.Narsi.Progress;
using Content.Server.RPSX.DarkForces.Narsi.Runes.Components;
using Content.Server.RPSX.DarkForces.Narsi.Runes.Events;
using Content.Server.RPSX.Utils;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using SpawnNarsiDoAfterEvent = Content.Shared.RPSX.DarkForces.Narsi.Runes.SpawnNarsiDoAfterEvent;

namespace Content.Server.RPSX.DarkForces.Narsi.Runes;

public sealed partial class NarsiRuneSystem
{
    [Dependency] private readonly NarsiCultProgressSystem _progressSystem = default!;

    private NarsiSummoningState _narsiSummoningState = NarsiSummoningState.Idle;

    private void InitSummoningNarsiRune()
    {
        SubscribeLocalEvent<NarsiSpawnRuneComponent, SpawnNarsiDoAfterEvent>(OnSpawnNarsiDoAfter);
    }

    private void OnRoundEndSummoningNarsi()
    {
        _narsiSummoningState = NarsiSummoningState.Idle;
    }

    private void SpawnNarsi(EntityUid rune, EntityUid user)
    {
        if (!StationUtils.IsEntityOnMainStationOnly(rune, EntityManager))
        {
            _popupSystem.PopupEntity("Призыв доступен только на станции", rune);
            return;
        }

        if (_narsiSummoningState == NarsiSummoningState.Summoning)
        {
            _popupSystem.PopupEntity("Кто-то уже призывает Нар'Си", rune);
            return;
        }

        if (HandleRuneInUse(rune))
            return;

        var count = EntityQuery<NarsiComponent>().ToList().Count;
        if (count > 0)
            return;

        if (!_progressSystem.CanSummonNarsi())
        {
            _popupSystem.PopupEntity("Вы не выполнили все требования для призыва Нар'Си", rune);
            return;
        }

        var doAfterEvent = new SpawnNarsiDoAfterEvent();
        var doAfterEventArgs = new DoAfterArgs(
            EntityManager,
            user: user,
            delay: TimeSpan.FromSeconds(240),
            @event: doAfterEvent,
            eventTarget: rune,
            target: null,
            used: rune
        );

        StartDoAfter(doAfterEventArgs, 5.0f);
        HandleRuneUsed(rune, true);
        RaiseLocalEvent(new NarsiSummoningStartEvent(rune));
    }

    private void OnSpawnNarsiDoAfter(EntityUid rune, NarsiSpawnRuneComponent component, SpawnNarsiDoAfterEvent args)
    {
        if (args.Handled)
            return;

        _narsiSummoningState = NarsiSummoningState.Idle;

        if (args.Cancelled)
        {
            RaiseLocalEvent(new NarsiSummoningCanceledEvent(rune));
            HandleRuneUsed(rune, false);
            return;
        }

        var transform = Transform(rune);
        var narsi = Spawn("Narsie", transform.Coordinates);

        RaiseLocalEvent(new NarsiSummoningEndEvent(rune, narsi));
        HandleRuneUsed(rune, false);
        args.Handled = true;
    }

    private enum NarsiSummoningState
    {
        Idle,
        Summoning
    }
}

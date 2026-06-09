using Content.Shared._Adventure.Synth;
using Content.Shared._Adventure.Synth.Components;
using Content.Shared.Alert;
using Content.Shared.Power.EntitySystems;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Robust.Client.Player;
using Robust.Shared.Timing;

namespace Content.Client._Adventure.Synth;

public sealed class SynthSystem : SharedSynthSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly SharedBatterySystem _battery = default!;

    private TimeSpan _nextAlertUpdate = TimeSpan.Zero;
    private static readonly TimeSpan AlertUpdateDelay = TimeSpan.FromSeconds(0.5f);

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;

        if (curTime < _nextAlertUpdate)
            return;

        _nextAlertUpdate = curTime + AlertUpdateDelay;

        var player = _player.LocalEntity;

        if (player == null)
            return;

        if (!HasComp<SynthComponent>(player.Value))
            return;

        UpdateBatteryAlert(player.Value);
    }

    private void UpdateBatteryAlert(EntityUid uid)
    {
        if (!TryComp<SynthComponent>(uid, out var synth))
            return;

        if (!TryComp<PowerCellSlotComponent>(uid, out var slotComp))
        {
            _alerts.ShowAlert(uid, synth.NoBatteryAlert);
            return;
        }

        if (!_powerCell.TryGetBatteryFromSlot((uid, slotComp), out var battery))
        {
            _alerts.ShowAlert(uid, synth.NoBatteryAlert);
            return;
        }

        var chargeLevel = (short)MathF.Round(_battery.GetCharge(battery.Value.AsNullable()) / battery.Value.Comp.MaxCharge * 10f);

        if (chargeLevel == 0 && _powerCell.HasDrawCharge((uid, null, slotComp)))
            chargeLevel = 1;

        _alerts.ShowAlert(uid, synth.BatteryAlert, chargeLevel);
    }
}

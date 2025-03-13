using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Power.Events;
using Content.Server.Stunnable.Components;
using Content.Server.Stunnable.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Examine;
using Content.Shared.Item;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;

namespace Content.Server._Adventure.Stunnable.Systems.Medical
{
    public sealed class StunDefibSystem : EntitySystem
    {
        [Dependency] private readonly SharedItemSystem _item = default!;
        [Dependency] private readonly SharedPopupSystem _popup = default!;
        [Dependency] private readonly BatterySystem _battery = default!;
        [Dependency] private readonly ItemToggleSystem _itemToggle = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<StunDefibComponent, ExaminedEvent>(OnExamined);
            SubscribeLocalEvent<StunDefibComponent, StaminaDamageOnHitAttemptEvent>(OnStaminaHitAttempt);
            SubscribeLocalEvent<StunDefibComponent, ItemToggleActivateAttemptEvent>(TryTurnOn);
            SubscribeLocalEvent<StunDefibComponent, ItemToggledEvent>(ToggleDone);
            SubscribeLocalEvent<StunDefibComponent, ChargeChangedEvent>(OnChargeChanged);
        }

        private void OnStaminaHitAttempt(Entity<StunDefibComponent> entity, ref StaminaDamageOnHitAttemptEvent args)
        {
            if (!_itemToggle.IsActivated(entity.Owner) ||
            !TryComp<BatteryComponent>(entity.Owner, out var battery) || !_battery.TryUseCharge(entity.Owner, entity.Comp.EnergyPerUse, battery))
            {
                args.Cancelled = true;
            }
        }

        private void OnExamined(Entity<StunDefibComponent> entity, ref ExaminedEvent args)
        {
            var onMsg = _itemToggle.IsActivated(entity.Owner)
            ? Loc.GetString("comp-stundefib-examined-on")
            : Loc.GetString("comp-stundefib-examined-off");
            args.PushMarkup(onMsg);

            if (TryComp<BatteryComponent>(entity.Owner, out var battery))
            {
                var count = (int) (battery.CurrentCharge / entity.Comp.EnergyPerUse);
                args.PushMarkup(Loc.GetString("melee-battery-examine", ("color", "yellow"), ("count", count)));
            }
        }

        private void ToggleDone(Entity<StunDefibComponent> entity, ref ItemToggledEvent args)
        {
            _item.SetHeldPrefix(entity.Owner, args.Activated ? "on" : "off");
        }

        private void TryTurnOn(Entity<StunDefibComponent> entity, ref ItemToggleActivateAttemptEvent args)
        {
            if (!TryComp<BatteryComponent>(entity, out var battery) || battery.CurrentCharge < entity.Comp.EnergyPerUse)
            {
                args.Cancelled = true;
                if (args.User != null)
                {
                    _popup.PopupEntity(Loc.GetString("stundefib-component-low-charge"), (EntityUid) args.User, (EntityUid) args.User);
                }
                return;
            }
        }

        private void SendPowerPulse(EntityUid target, EntityUid? user, EntityUid used)
        {
            RaiseLocalEvent(target, new PowerPulseEvent()
            {
                Used = used,
                User = user
            });
        }

        private void OnChargeChanged(Entity<StunDefibComponent> entity, ref ChargeChangedEvent args)
        {
            if (TryComp<BatteryComponent>(entity.Owner, out var battery) &&
                battery.CurrentCharge < entity.Comp.EnergyPerUse)
            {
                _itemToggle.TryDeactivate(entity.Owner, predicted: false);
            }
        }
    }
}

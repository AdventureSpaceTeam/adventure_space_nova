using Content.Server._Adventure.DarkForces.Saint.Chaplain.Components;
using Content.Shared._Adventure.DarkForces.Narsi.Roles;
using Content.Shared.Item;
using Content.Shared.Popups;

namespace Content.Server._Adventure.DarkForces.Narsi.Cultist.Gear;

public sealed class NarsiCultistGearSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NarsiCultistGearComponent, GettingPickedUpAttemptEvent>(OnTryPickedUpItem);
    }

    private void OnTryPickedUpItem(EntityUid uid, NarsiCultistGearComponent component, GettingPickedUpAttemptEvent args)
    {
        var user = args.User;

        if (HasComp<NarsiCultistComponent>(user))
            return;

        if (HasComp<ChaplainComponent>(user))
        {
            _popup.PopupCursor("Вам удается поднять этот предмет, но вы чувствуете энергию сил тьмы", user);
            return;
        }

        args.Cancel();
        _popup.PopupCursor("Вам не удается поднять этот предмет...", user);
    }
}

using Content.Server.AdventureSpace.Medical.Disease.Components;
using Content.Server.DoAfter;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Shared.AdventureSpace.Medical.Diseases.Diagnosis.Swab;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Inventory;

namespace Content.Server.AdventureSpace.Medical.Disease.Diagnosis.Swab;

public sealed class DiseaseSwabSystem : EntitySystem
{
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly InventorySystem _inventorySystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DiseaseSwabComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<DiseaseSwabComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<DiseaseSwabComponent, DiseaseSwabDoAfterEvent>(OnSwabDoAfter);
    }

    private void OnSwabDoAfter(EntityUid uid, DiseaseSwabComponent component, DiseaseSwabDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Target is not { } target)
            return;

        args.Handled = true;
        component.Used = true;

        _popupSystem.PopupEntity(Loc.GetString("diseases-swab-swabbed"), target, args.Args.User);

        if (!TryComp<DiseaseTargetComponent>(target, out var diseaseTarget))
            return;

        foreach (var (key, value) in diseaseTarget.Diseases)
        {
            component.DiseasesPrototypesIds.Add(key);
        }
    }

    private void OnExamined(EntityUid uid, DiseaseSwabComponent swab, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        args.PushMarkup(swab.Used ? Loc.GetString("diseases-swab-used") : Loc.GetString("diseases-swab-unused"));
    }

    private void OnAfterInteract(EntityUid uid, DiseaseSwabComponent swab, AfterInteractEvent args)
    {
        if (args.Target is not { } target || !args.CanReach)
            return;

        if (!HasComp<DiseaseTargetComponent>(args.Target))
            return;

        if (swab.Used)
        {
            _popupSystem.PopupEntity(Loc.GetString("diseases-swab-used"), args.User, args.User);
            return;
        }

        if (IsMouthBlocked(target))
        {
            _popupSystem.PopupEntity(Loc.GetString("diseases-swab-blocked"), args.User, args.User);
            return;
        }

        var doAfterArgs = new DoAfterArgs(
            EntityManager,
            args.User,
            swab.SwabDelay,
            new DiseaseSwabDoAfterEvent(),
            uid,
            target: target,
            used: uid)
        {
            BreakOnMove = true,
            NeedHand = true,
        };
        _doAfterSystem.TryStartDoAfter(doAfterArgs);

        args.Handled = true;
    }

    private bool IsMouthBlocked(EntityUid target)
    {
        if (!_inventorySystem.TryGetSlotEntity(target, "mask", out var maskUid))
            return false;

        return TryComp<IngestionBlockerComponent>(maskUid, out var blocker) && blocker.Enabled;
    }
}

using Content.Server.PowerCell;
using Content.Server.Silicons.Laws;
using Content.Shared.AdventureSpace.Roles.StationAI.Components;
using Content.Shared.AdventureSpace.Roles.StationAI.UI;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Silicons.Borgs.Components;

namespace Content.Server.AdventureSpace.Roles.StationAI;

public sealed partial class StationAISystem
{
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly SiliconLawSystem _siliconLawSystem = default!;

    private void InitializeBorgs()
    {
        Subs.BuiEvents<StationAIGhostComponent>(
            uiKey: StationAIBorgUiKey.Key,
            subscriber: subs =>
            {
                subs.Event<StationAIRequestBorgsList>(OnBorgsListRequested);
                subs.Event<StationAIBorgCameraRequest>(OnBorgCameraRequest);
            }
        );
    }

    private void OnBorgCameraRequest(Entity<StationAIGhostComponent> ent, ref StationAIBorgCameraRequest args)
    {
        var borg = GetEntity(args.Borg);
        if (!borg.IsValid() || !TryComp<ItemToggleComponent>(borg, out var borgChassis) || !borgChassis.Activated)
            return;

        if (ent.Comp.ActiveCamera != EntityUid.Invalid)
            DropCamera(ent);

        AttachCamera(ent, borg);
    }

    private void OnBorgsListRequested(EntityUid uid, StationAIGhostComponent component, StationAIRequestBorgsList args)
    {
        if (!_userInterfaceSystem.HasUi(uid, StationAIBorgUiKey.Key))
            return;

        var entityTransform = Transform(uid);
        var borgsList = new List<StationAIBorgUIModel>();

        var borgs = EntityManager
            .EntityQueryEnumerator<BorgChassisComponent, ItemToggleComponent, TransformComponent, MetaDataComponent>();
        while (borgs.MoveNext(out var borg, out var borgComp, out var itemToggle, out var transform, out var meta))
        {
            if (transform.MapID != entityTransform.MapID)
                continue;

            if (!itemToggle.Activated)
                continue;

            borgsList.Add(GetBorgUIModel(borg, meta));
        }

        var state = new StationAIBorgInterfaceState(borgsList);
        _userInterfaceSystem.SetUiState(uid, StationAIBorgUiKey.Key, state);
    }

    private StationAIBorgUIModel GetBorgUIModel(EntityUid borg, MetaDataComponent borgMeta)
    {
        var coordinates = _transform.GetMoverCoordinates(borg);
        var coordinatesText = $"X: {coordinates.X}; Y: {coordinates.Y}";

        var batteryPercent = 0f;
        if (_powerCell.TryGetBatteryFromSlot(borg, out var battery))
            batteryPercent = battery.CurrentCharge / battery.MaxCharge;

        return new StationAIBorgUIModel(
            borg: GetNetEntity(borg),
            name: borgMeta.EntityName,
            coordinates: coordinatesText,
            percent: batteryPercent,
            laws: _siliconLawSystem.GetLaws(borg)
        );
    }
}

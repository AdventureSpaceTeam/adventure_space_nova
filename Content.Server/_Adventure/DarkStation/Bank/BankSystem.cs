using Content.Server.Access.Systems;
using Content.Server.Administration.Logs;
using Content.Server.Database;
using Content.Server.Mind;
using Content.Server.Popups;
using Content.Server.Preferences.Managers;
using Content.Server.Stack;
using Content.Server.Station.Systems;
using Content.Server.StationRecords.Systems;
using Content.Shared.Containers.ItemSlots;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;

namespace Content.Server._Adventure.Bank;

public sealed partial class BankSystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly IServerDbManager _dbManager = default!;
    [Dependency] private readonly IdCardSystem _idCardSystem = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IServerPreferencesManager _prefsManager = default!;
    [Dependency] private readonly StackSystem _stackSystem = default!;
    [Dependency] private readonly StationRecordsSystem _stationRecordsSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;

    private ISawmill _log = default!;

    public override void Initialize()
    {
        base.Initialize();
        _log = Logger.GetSawmill("bank");

        InitializeATM();
        InitializeAccount();
    }
}

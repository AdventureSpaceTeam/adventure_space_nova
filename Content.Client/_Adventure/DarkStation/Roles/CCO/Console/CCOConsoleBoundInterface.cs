using Content.Shared._Adventure.Roles.CCO;

namespace Content.Client._Adventure.Roles.CCO.Console;

public sealed class CcoConsoleBoundInterface : BoundUserInterface
{
    private CcoConsoleWindow? _window;

    public CcoConsoleBoundInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        if (_window is not null)
            return;

        _window = new CcoConsoleWindow(this);
        _window.OnClose += Close;
        _window.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
            return;

        _window?.Close();
        _window?.Dispose();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not CcoConsoleUIState ccoConsoleUIState || _window == null)
            return;

        _window.UpdateState(ccoConsoleUIState);
    }

    public void OnEmergencyShuttlePressed(EmergencyShuttleState state)
    {
        switch (state)
        {
            case EmergencyShuttleState.Idle:
                SendMessage(new CcoConsoleSendEmergencyShuttleMessage());
                break;
            case EmergencyShuttleState.OnWay:
                SendMessage(new CcoConsoleCancelEmergencyShuttleMessage());
                break;
        }
    }

    public void SendSpecialSquad(string prototypeId)
    {
        SendMessage(new CcoConsoleSendSpecialSquadMessage(prototypeId));
    }

    public void SendAnnouncement(string message)
    {
        SendMessage(new CcoConsoleSendAnnouncementMessage(message));
    }

    public void OpenManifest()
    {
        SendMessage(new CcoConsoleOpenCrewManifestMessage());
    }

    public void CrewMemberApplySalaryBonus(int bonus, uint record)
    {
        SendMessage(new CcoConsoleCrewMemberSalaryBonusMessage(bonus, record));
    }

    public void CrewMemberApplySalaryPenalty(int penalty, uint record)
    {
        SendMessage(new CcoConsoleCrewMemberSalaryPenaltyMessage(penalty, record));
    }

    public void OnStationSelected(NetEntity station)
    {
        SendMessage(new CcoConsoleStationSelected(station));
    }
}

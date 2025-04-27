using Content.Shared._Adventure.Roles.CCO;
using Robust.Shared.Utility;

namespace Content.Client._Adventure.Roles.CCO.Console;

public sealed partial class CcoConsoleWindow
{
    private void SetupEmergencyShuttle()
    {
        StationCallEmergencyShuttle.OnPressed += _ =>
        {
            if (_state?.ConsoleBase.SelectedStation is not { } selectedStation)
                return;

            var station = _state.Stations.AvailableStation[selectedStation];
            _bui.OnEmergencyShuttlePressed(station.EmergencyShuttle.ShuttleState);
        };
    }

    private void BindEmergencyShuttle(CcoConsoleEmergencyShuttle emergencyShuttle)
    {
        var status = "Потеря связи";
        var buttonText = "Отправить эвакуационный шаттл";

        switch (emergencyShuttle.ShuttleState)
        {
            case EmergencyShuttleState.Idle:
                status = "Готов к отправке";
                buttonText = "Отправить эвакуационный шаттл";
                break;
            case EmergencyShuttleState.Arrived:
                status = "Прибыл на станцию";
                buttonText = "Недоступно";
                break;
            case EmergencyShuttleState.OnWay:
                status = "В пути на станцию";
                buttonText = "Отозвать эвакуационный шаттл";
                break;
            case EmergencyShuttleState.OnWayCentcom:
                status = "В пути на центком";
                buttonText = "Недоступно";
                break;
            case EmergencyShuttleState.Unknown:
                status = "Потеря связи";
                buttonText = "Недоступно";
                break;
        }

        EmergencyShuttle.SetMessage(FormattedMessage.FromMarkup(status));
        StationCallEmergencyShuttle.Disabled = emergencyShuttle.ShuttleState != EmergencyShuttleState.Idle &&
                                               emergencyShuttle.ShuttleState != EmergencyShuttleState.OnWay;
        StationCallEmergencyShuttle.Text = buttonText;
    }
}

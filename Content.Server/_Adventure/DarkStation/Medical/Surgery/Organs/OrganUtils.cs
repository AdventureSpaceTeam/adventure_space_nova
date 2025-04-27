using Content.Shared.Body.Organ;

namespace Content.Server._Adventure.Medical.Surgery.Organs;

public static class OrganUtils
{
    public static string GetOrganColoredState(OrganCondition condition)
    {
        var color = condition switch
        {
            OrganCondition.Good => "#008000",
            OrganCondition.Warning => "#FFFF00",
            OrganCondition.Critical => "#FF8C00",
            OrganCondition.Failure => "#7C0000",
            _ => "#FFFFFF",
        };

        var conditionText = condition switch
        {
            OrganCondition.Good => "Отлично",
            OrganCondition.Warning => "Нормально",
            OrganCondition.Critical => "Критическое состояние",
            OrganCondition.Failure => "Отказ работы",
            _ => "Не найдено",
        };

        return $"[color={color}]{conditionText}[/color]";
    }
}

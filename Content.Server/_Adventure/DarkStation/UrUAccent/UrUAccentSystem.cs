using System.Text.RegularExpressions;
using Content.Server.Speech;
using Robust.Shared.Random;

namespace Content.Server._Adventure.UrUAccent;

public sealed class UrUAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<UrUAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, UrUAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // у => ууу
        message = Regex.Replace(
            message,
            "у+",
            _random.Pick(new List<string> { "у", "уу", "ууу" })
        );
        // У => УУУ
        message = Regex.Replace(
            message,
            "У+",
            _random.Pick(new List<string> { "У", "УУ", "УУУ" })
        );
        // р => ррр
        message = Regex.Replace(
            message,
            "р+",
            _random.Pick(new List<string> { "р", "рр", "ррр" })
        );
        // Р => РРР
        message = Regex.Replace(
            message,
            "Р+",
            _random.Pick(new List<string> { "Р", "РР", "РРР" })
        );
        // в => ввв 
        message = Regex.Replace(
            message,
            "в+",
            _random.Pick(new List<string> { "в", "вв", "ввв" })
        );
        // В => ВВВ 
        message = Regex.Replace(
            message,
            "В+",
            _random.Pick(new List<string> { "В", "ВВ", "ВВВ" })
        );
        // а => ааа 
        message = Regex.Replace(
            message,
            "а+",
            _random.Pick(new List<string> { "а", "аа", "ааа" })
        );
        // А => ААА 
        message = Regex.Replace(
            message,
            "А+",
            _random.Pick(new List<string> { "А", "АА", "ААА" })
        );
        // н => ннн 
        message = Regex.Replace(
            message,
            "н+",
            _random.Pick(new List<string> { "н", "нн", "ннн" })
        );
        // Н => ННН 
        message = Regex.Replace(
            message,
            "Н+",
            _random.Pick(new List<string> { "Н", "НН", "ННН" })
        );
        // м => ммм 
        message = Regex.Replace(
            message,
            "м+",
            _random.Pick(new List<string> { "м", "мм", "ммм" })
        );
        // М => МММ 
        message = Regex.Replace(
            message,
            "М+",
            _random.Pick(new List<string> { "М", "ММ", "МММ" })
        );
        // г => ггг 
        message = Regex.Replace(
            message,
            "г+",
            _random.Pick(new List<string> { "г", "гг", "ггг" })
        );
        // Г => ГГГ 
        message = Regex.Replace(
            message,
            "Г+",
            _random.Pick(new List<string> { "Г", "ГГ", "ГГГ" })
        );

        args.Message = message;
    }
}

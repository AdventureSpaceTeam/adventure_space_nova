using System.Text.RegularExpressions;
using Content.Shared.Speech;
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

    public static readonly Regex regYLower = new("у+", RegexOptions.Compiled);
    public static readonly Regex regYUpper = new("У+", RegexOptions.Compiled);
    public static readonly Regex regRLower = new("р+", RegexOptions.Compiled);
    public static readonly Regex regRUpper = new("Р+", RegexOptions.Compiled);
    public static readonly Regex regVLower = new("в+", RegexOptions.Compiled);
    public static readonly Regex regVUpper = new("В+", RegexOptions.Compiled);
    public static readonly Regex regALower = new("а+", RegexOptions.Compiled);
    public static readonly Regex regAUpper = new("А+", RegexOptions.Compiled);
    public static readonly Regex regNLower = new("н+", RegexOptions.Compiled);
    public static readonly Regex regNUpper = new("Н+", RegexOptions.Compiled);
    public static readonly Regex regMLower = new("м+", RegexOptions.Compiled);
    public static readonly Regex regMUpper = new("М+", RegexOptions.Compiled);
    public static readonly Regex regGLower = new("г+", RegexOptions.Compiled);
    public static readonly Regex regGUpper = new("Г+", RegexOptions.Compiled);

    private void OnAccent(EntityUid uid, UrUAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        message = regYLower.Replace(message, _random.Pick(new List<string> { "у", "уу", "ууу" }));
        message = regYUpper.Replace(message, _random.Pick(new List<string> { "У", "УУ", "УУУ" }));
        message = regRLower.Replace(message, _random.Pick(new List<string> { "р", "рр", "ррр" }));
        message = regRUpper.Replace(message, _random.Pick(new List<string> { "Р", "РР", "РРР" }));
        message = regVLower.Replace(message, _random.Pick(new List<string> { "в", "вв", "ввв" }));
        message = regVUpper.Replace(message, _random.Pick(new List<string> { "В", "ВВ", "ВВВ" }));
        message = regALower.Replace(message, _random.Pick(new List<string> { "а", "аа", "ааа" }));
        message = regAUpper.Replace(message, _random.Pick(new List<string> { "А", "АА", "ААА" }));
        message = regNLower.Replace(message, _random.Pick(new List<string> { "н", "нн", "ннн" }));
        message = regNUpper.Replace(message, _random.Pick(new List<string> { "Н", "НН", "ННН" }));
        message = regMLower.Replace(message, _random.Pick(new List<string> { "м", "мм", "ммм" }));
        message = regMUpper.Replace(message, _random.Pick(new List<string> { "М", "ММ", "МММ" }));
        message = regGLower.Replace(message, _random.Pick(new List<string> { "г", "гг", "ггг" }));
        message = regGUpper.Replace(message, _random.Pick(new List<string> { "Г", "ГГ", "ГГГ" }));

        args.Message = message;
    }
}

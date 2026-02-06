using System.Text.RegularExpressions;
using Content.Shared.Speech;
using Robust.Shared.Random;

namespace Content.Server._Adventure.SnakeAccent;

public sealed class SnakeAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SnakeAccentComponent, AccentGetEvent>(OnAccent);
    }

    // NOTE(c4ll): Was vibecoded, because I don't want to write macros for this.
    // TODO(c4ll): Make it one big regex
    public static readonly Regex regFLower = new("ф+", RegexOptions.Compiled);
    public static readonly Regex regFUpper = new("Ф+", RegexOptions.Compiled);
    public static readonly Regex regVLower = new("в+", RegexOptions.Compiled);
    public static readonly Regex regVUpper = new("В+", RegexOptions.Compiled);
    public static readonly Regex regTsLower = new("ц+", RegexOptions.Compiled);
    public static readonly Regex regTsUpper = new("Ц+", RegexOptions.Compiled);
    public static readonly Regex regKhLower = new("х+", RegexOptions.Compiled);
    public static readonly Regex regKhUpper = new("Х+", RegexOptions.Compiled);
    public static readonly Regex regJLower = new("й+", RegexOptions.Compiled);
    public static readonly Regex regJUpper = new("Й+", RegexOptions.Compiled);
    public static readonly Regex regSLower = new("с+", RegexOptions.Compiled);
    public static readonly Regex regSUpper = new("С+", RegexOptions.Compiled);
    public static readonly Regex regZLower = new("з+", RegexOptions.Compiled);
    public static readonly Regex regZUpper = new("З+", RegexOptions.Compiled);
    public static readonly Regex regShLower = new("ш+", RegexOptions.Compiled);
    public static readonly Regex regShUpper = new("Ш+", RegexOptions.Compiled);
    public static readonly Regex regChLower = new("ч+", RegexOptions.Compiled);
    public static readonly Regex regChUpper = new("Ч+", RegexOptions.Compiled);
    public static readonly Regex regZhLower = new("ж+", RegexOptions.Compiled);
    public static readonly Regex regZhUpper = new("Ж+", RegexOptions.Compiled);
    public static readonly Regex regSchLower = new("щ+", RegexOptions.Compiled);
    public static readonly Regex regSchUpper = new("Щ+", RegexOptions.Compiled);

    private void OnAccent(EntityUid uid, SnakeAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        message = regFLower.Replace(message, _random.Pick(new List<string> { "фф", "ффф", "фффф", "ффффф" }));
        message = regFUpper.Replace(message, _random.Pick(new List<string> { "ФФ", "ФФФ", "ФФФФ", "ФФФФФ" }));
        message = regVLower.Replace(message, _random.Pick(new List<string> { "фф", "ффф", "фффф", "ффффф" }));
        message = regVUpper.Replace(message, _random.Pick(new List<string> { "ФФ", "ФФФ", "ФФФФ", "ФФФФФ" }));
        message = regTsLower.Replace(message, _random.Pick(new List<string> { "сс", "ссс", "сссс", "сссс" }));
        message = regTsUpper.Replace(message, _random.Pick(new List<string> { "СС", "ССС", "СССС", "СССС" }));
        message = regKhLower.Replace(message, _random.Pick(new List<string> { "хх", "ххх", "хххх", "ххххх" }));
        message = regKhUpper.Replace(message, _random.Pick(new List<string> { "ХХ", "ХХХ", "ХХХХ", "ХХХХХ" }));
        message = regJLower.Replace(message, _random.Pick(new List<string> { "йй", "ййй", "йййй", "ййййй" }));
        message = regJUpper.Replace(message, _random.Pick(new List<string> { "ЙЙ", "ЙЙЙ", "ЙЙЙЙ", "ЙЙЙЙЙ" }));
        message = regSLower.Replace(message, _random.Pick(new List<string>() { "сс", "ссс", "сссс", "ссссс" }));
        message = regSUpper.Replace(message, _random.Pick(new List<string>() { "СС", "ССС", "СССС", "ССССС" }));
        message = regZLower.Replace(message, _random.Pick(new List<string>() { "сс", "ссс", "сссс", "ссссс" }));
        message = regZUpper.Replace(message, _random.Pick(new List<string>() { "СС", "ССС", "СССС", "ССССС" }));
        message = regShLower.Replace(message, _random.Pick(new List<string>() { "шш", "шшш", "шшшш", "шшшшш", "щщ", "щщщ", "щщщщ", "щщщщщ" }));
        message = regShUpper.Replace(message, _random.Pick(new List<string>() { "ШШ", "ШШШ", "ШШШШ", "ШШШШШ", "ЩЩ", "ЩЩЩ", "ЩЩЩЩ", "ЩЩЩЩЩ" }));
        message = regChLower.Replace(message, _random.Pick(new List<string>() { "шш", "шшш", "шшшш", "шшшшш", "щщ", "щщщ", "щщщщ", "щщщщщ" }));
        message = regChUpper.Replace(message, _random.Pick(new List<string>() { "ШШ", "ШШШ", "ШШШШ", "ШШШШШ", "ЩЩ", "ЩЩЩ", "ЩЩЩЩ", "ЩЩЩЩЩ" }));
        message = regZhLower.Replace(message, _random.Pick(new List<string>() { "шш", "шшш", "шшшш", "шшшшш", "щщ", "щщщ", "щщщщ", "щщщщщ" }));
        message = regZhUpper.Replace(message, _random.Pick(new List<string>() { "ШШ", "ШШШ", "ШШШШ", "ШШШШШ", "ЩЩ", "ЩЩЩ", "ЩЩЩЩ", "ЩЩЩЩЩ" }));
        message = regSchLower.Replace(message, _random.Pick(new List<string>() { "шш", "шшш", "шшшш", "шшшшш", "щщ", "щщщ", "щщщщ", "щщщщщ" }));
        message = regSchUpper.Replace(message, _random.Pick(new List<string>() { "ШШ", "ШШШ", "ШШШШ", "ШШШШШ", "ЩЩ", "ЩЩЩ", "ЩЩЩЩ", "ЩЩЩЩЩ" }));

        args.Message = message;
    }
}

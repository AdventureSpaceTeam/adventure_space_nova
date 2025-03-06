using System.Text.RegularExpressions;
using Content.Server.Speech;
using Robust.Shared.Random;

namespace Content.Server._Adventure.Races.SnakeAccent;

public sealed class SnakeAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SnakeAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, SnakeAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // ф => ффф 
        message = Regex.Replace(
            message,
            "ф+",
            _random.Pick(new List<string> { "фф", "ффф", "фффф", "ффффф" })
        );
        // Ф => ФФФ 
        message = Regex.Replace(
            message,
            "Ф+",
            _random.Pick(new List<string> { "ФФ", "ФФФ", "ФФФФ", "ФФФФФ" })
        );
        // в => ффф 
        message = Regex.Replace(
            message,
            "в+",
            _random.Pick(new List<string> { "фф", "ффф", "фффф", "ффффф" })
        );
        // В => ФФФ 
        message = Regex.Replace(
            message,
            "В+",
            _random.Pick(new List<string> { "ФФ", "ФФФ", "ФФФФ", "ФФФФФ" })
        );
        // ц => ссс 
        message = Regex.Replace(
            message,
            "ц+",
            _random.Pick(new List<string> { "сс", "ссс", "сссс", "сссс" })
        );
        // Ц => ССС 
        message = Regex.Replace(
            message,
            "Ц+",
            _random.Pick(new List<string> { "СС", "ССС", "СССС", "СССС" })
        );
        // х => ххх 
        message = Regex.Replace(
            message,
            "х+",
            _random.Pick(new List<string> { "хх", "ххх", "хххх", "ххххх" })
        );
        // Х => ХХХ 
        message = Regex.Replace(
            message,
            "Х+",
            _random.Pick(new List<string> { "ХХ", "ХХХ", "ХХХХ", "ХХХХХ" })
        );
        // й => ййй 
        message = Regex.Replace(
            message,
            "й+",
            _random.Pick(new List<string> { "йй", "ййй", "йййй", "ййййй" })
        );
        // Й => ЙЙЙ
        message = Regex.Replace(
            message,
            "Й+",
            _random.Pick(new List<string> { "ЙЙ", "ЙЙЙ", "ЙЙЙЙ", "ЙЙЙЙЙ" })
        );
        // c => ссс
        message = Regex.Replace(
            message,
            "с+",
            _random.Pick(new List<string>() { "сс", "ссс", "сссс", "ссссс" })
        );
        // С => CCC
        message = Regex.Replace(
            message,
            "С+",
            _random.Pick(new List<string>() { "СС", "ССС", "СССС", "ССССС" })
        );
        // з => ссс
        message = Regex.Replace(
            message,
            "з+",
            _random.Pick(new List<string>() { "сс", "ссс", "сссс", "ссссс" })
        );
        // З => CCC
        message = Regex.Replace(
            message,
            "З+",
            _random.Pick(new List<string>() { "СС", "ССС", "СССС", "ССССС" })
        );
        // ш => шшш,щщщ
        message = Regex.Replace(
            message,
            "ш+",
            _random.Pick(new List<string>() { "шш", "шшш", "шшшш", "шшшшш", "щщ", "щщщ", "щщщщ", "щщщщщ" }) 
        );
        // Ш => ШШШ,ЩЩЩ
        message = Regex.Replace(
            message,
            "Ш+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ШШШШ", "ШШШШШ", "ЩЩ", "ЩЩЩ", "ЩЩЩЩ", "ЩЩЩЩЩ" }) 
        );
        // ч => щщщ,шшш
        message = Regex.Replace(
            message,
            "ч+",
            _random.Pick(new List<string>() { "шш", "шшш", "шшшш", "шшшшш", "щщ", "щщщ", "щщщщ", "щщщщщ" }) 
        );
        // Ч => ЩЩЩ,ШШШ
        message = Regex.Replace(
            message,
            "Ч+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ШШШШ", "ШШШШШ", "ЩЩ", "ЩЩЩ", "ЩЩЩЩ", "ЩЩЩЩЩ" }) 
        );
        // ж => жжж 
        message = Regex.Replace(
            message,
            "ж+",
            _random.Pick(new List<string>() { "шш", "шшш", "шшшш", "шшшшш", "щщ", "щщщ", "щщщщ", "щщщщщ" })
        );
        // Ж => ЖЖЖ 
        message = Regex.Replace(
            message,
            "Ж+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ШШШШ", "ШШШШШ", "ЩЩ", "ЩЩЩ", "ЩЩЩЩ", "ЩЩЩЩЩ" })
        );
        // щ => щщщ,шшш 
        message = Regex.Replace(
            message,
            "щ+",
            _random.Pick(new List<string>() { "шш", "шшш", "шшшш", "шшшшш", "щщ", "щщщ", "щщщщ", "щщщщщ" })
        );
        // Щ => ЩЩЩ,ШШШ 
        message = Regex.Replace(
            message,
            "Щ+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ШШШШ", "ШШШШШ", "ЩЩ", "ЩЩЩ", "ЩЩЩЩ", "ЩЩЩЩЩ" })
        );

        args.Message = message;
    }
}

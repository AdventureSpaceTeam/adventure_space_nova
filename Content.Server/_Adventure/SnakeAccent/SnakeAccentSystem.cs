using System.Text.RegularExpressions;
using Content.Server.Speech;
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

    private void OnAccent(EntityUid uid, SnakeAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // к => кххх 
        message = Regex.Replace(
            message,
            "к+",
            _random.Pick(new List<string> { "кхх", "кххх" })
        );
        // К => КХХХ 
        message = Regex.Replace(
            message,
            "К+",
            _random.Pick(new List<string> { "КХХ", "КХХХ" })
        );
        // ф => ффф 
        message = Regex.Replace(
            message,
            "ф+",
            _random.Pick(new List<string> { "фф", "ффф" })
        );
        // Ф => ФФФ 
        message = Regex.Replace(
            message,
            "Ф+",
            _random.Pick(new List<string> { "ФФ", "ФФФ" })
        );
        // ц => ссс 
        message = Regex.Replace(
            message,
            "ц+",
            _random.Pick(new List<string> { "сс", "ссс" })
        );
        // Ц => ССС 
        message = Regex.Replace(
            message,
            "Ц+",
            _random.Pick(new List<string> { "СС", "ССС" })
        );
        // г => гххх 
        message = Regex.Replace(
            message,
            "г+",
            _random.Pick(new List<string> { "гхх", "гххх" })
        );
        // Г => ГХХХ 
        message = Regex.Replace(
            message,
            "Г+",
            _random.Pick(new List<string> { "ГХХ", "ГХХХ" })
        );
        // х => ххх 
        message = Regex.Replace(
            message,
            "х+",
            _random.Pick(new List<string> { "хх", "ххх" })
        );
        // Х => ХХХ 
        message = Regex.Replace(
            message,
            "Х+",
            _random.Pick(new List<string> { "ХХ", "ХХХ" })
        );
        // й => ййй 
        message = Regex.Replace(
            message,
            "й+",
            _random.Pick(new List<string> { "йй", "ййй" })
        );
        // Й => ЙЙЙ
        message = Regex.Replace(
            message,
            "Й+",
            _random.Pick(new List<string> { "ЙЙ", "ЙЙЙ" })
        );
        // я => йа
        message = Regex.Replace(
            message,
            "я+",
            _random.Pick(new List<string> { "ййа", "йййа" })
        );
        // Я => ЙА 
        message = Regex.Replace(
            message,
            "Я+",
            _random.Pick(new List<string> { "ЙЙА", "ЙЙЙА" })
        );
        // ё => йо
        message = Regex.Replace(
            message,
            "ё+",
            _random.Pick(new List<string> { "ййо", "йййо" })
        );
        // Ё => ЙО 
        message = Regex.Replace(
            message,
            "Ё+",
            _random.Pick(new List<string> { "ЙЙО", "ЙЙЙО" })
        );
        // ю => йу
        message = Regex.Replace(
            message,
            "ю+",
            _random.Pick(new List<string> { "ййу", "йййу" })
        );
        // Ю => ЙУ 
        message = Regex.Replace(
            message,
            "Ю+",
            _random.Pick(new List<string> { "ЙЙУ", "ЙЙЙУ" })
        );
        // е => йэ
        message = Regex.Replace(
            message,
            "е+",
            _random.Pick(new List<string> { "ййэ", "йййэ" })
        );
        // Е => ЙЭ 
        message = Regex.Replace(
            message,
            "Е+",
            _random.Pick(new List<string> { "ЙЙЭ", "ЙЙЙЭ" })
        );
        // и => йи
        message = Regex.Replace(
            message,
            "и+",
            _random.Pick(new List<string> { "ййи", "йййи" })
        );
        // И => ЙИ
        message = Regex.Replace(
            message,
            "И+",
            _random.Pick(new List<string> { "ЙЙИ", "ЙЙЙИ" })
        );
        // c => ссс
        message = Regex.Replace(
            message,
            "с+",
            _random.Pick(new List<string>() { "сс", "ссс" })
        );
        // С => CCC
        message = Regex.Replace(
            message,
            "С+",
            _random.Pick(new List<string>() { "СС", "ССС" })
        );
        // з => ссс
        message = Regex.Replace(
            message,
            "з+",
            _random.Pick(new List<string>() { "сс", "ссс" })
        );
        // З => CCC
        message = Regex.Replace(
            message,
            "З+",
            _random.Pick(new List<string>() { "СС", "ССС" })
        );
        // ш => шшш,щщщ
        message = Regex.Replace(
            message,
            "ш+",
            _random.Pick(new List<string>() { "шш", "шшш", "щщ", "щщщ" }) 
        );
        // Ш => ШШШ,ЩЩЩ
        message = Regex.Replace(
            message,
            "Ш+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ЩЩ", "ЩЩЩ" }) 
        );
        // ч => щщщ,шшш
        message = Regex.Replace(
            message,
            "ч+",
            _random.Pick(new List<string>() { "щщ", "щщщ", "шш", "шшш" }) 
        );
        // Ч => ЩЩЩ,ШШШ
        message = Regex.Replace(
            message,
            "Ч+",
            _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ", "ШШ", "ШШШ" }) 
        );
        // ж => жжж 
        message = Regex.Replace(
            message,
            "ж+",
            _random.Pick(new List<string>() { "шш", "шшш", "щщ", "щщщ" })
        );
        // Ж => ЖЖЖ 
        message = Regex.Replace(
            message,
            "Ж+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ЩЩ", "ЩЩЩ" })
        );
        // щ => щщщ,шшш 
        message = Regex.Replace(
            message,
            "щ+",
            _random.Pick(new List<string>() { "щщ", "щщщ", "шш", "шшш" })
        );
        // Щ => ЩЩЩ,ШШШ 
        message = Regex.Replace(
            message,
            "Щ+",
            _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ", "ШШ", "ШШШ" })
        );

        args.Message = message;
    }
}

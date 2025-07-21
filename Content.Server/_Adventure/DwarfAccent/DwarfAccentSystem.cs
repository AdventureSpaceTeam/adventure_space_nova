using System.Text.RegularExpressions;
using Content.Server.Speech;
using Robust.Shared.Random;

namespace Content.Server._Adventure.DwarfAccent;

public sealed class DwarfAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DwarfAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, DwarfAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // э => йе 
        message = Regex.Replace(
            message,
            "э+",
            _random.Pick(new List<string> { "йе" })
        );
        // Э => ЙЕ
        message = Regex.Replace(
            message,
            "Э+",
            _random.Pick(new List<string> { "ЙЕ" })
        );
        // е => э
        message = Regex.Replace(
            message,
            "е+",
            _random.Pick(new List<string> { "э" })
        );
        // Е => Э 
        message = Regex.Replace(
            message,
            "Е+",
            _random.Pick(new List<string> { "Э" })
        );
        // и => ые 
        message = Regex.Replace(
            message,
            "и+",
            _random.Pick(new List<string> { "ые" })
        );
        // И => ЫЕ
        message = Regex.Replace(
            message,
            "И+",
            _random.Pick(new List<string> { "ЫЕ" })
        );

        args.Message = message;
    }
}

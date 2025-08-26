using System.Text.RegularExpressions;
using Content.Server._Corvax.Speech.Components;
using Content.Shared.Speech;
using Robust.Shared.Random;

namespace Content.Server._Corvax.Speech.EntitySystems;

public sealed class GrowlingAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GrowlingAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, GrowlingAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // r => rrr
        message = Regex.Replace(
            message,
            "r+",
            _random.Pick(new List<string> { "rr", "rrr" })
        );
        // R => RRR
        message = Regex.Replace(
            message,
            "R+",
            _random.Pick(new List<string> { "RR", "RRR" })
        );

        // р => ррр
        message = Regex.Replace(
            message,
            "р+",
            _random.Pick(new List<string> { "рр", "ррр" })
        );
        // Р => РРР
        message = Regex.Replace(
            message,
            "Р+",
            _random.Pick(new List<string> { "РР", "РРР" })
        );

        // Adventure-Localization-Start
        // в => ввв
        message = Regex.Replace(
            message,
            "в+",
            _random.Pick(new List<string> { "вв", "ввв" })
        );
        // В => ВВВ
        message = Regex.Replace(
            message,
            "В+",
            _random.Pick(new List<string> { "ВВ", "ВВВ" })
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
        // Adventure-Localization-End

        args.Message = message;
    }
}

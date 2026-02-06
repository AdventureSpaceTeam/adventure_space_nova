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

    // Adventure zero errors begin
    public static readonly Regex regRLower = new("r+", RegexOptions.Compiled);
    public static readonly Regex regRUpper = new("R+", RegexOptions.Compiled);
    public static readonly Regex regPLower = new("р+", RegexOptions.Compiled);
    public static readonly Regex regPUpper = new("Р+", RegexOptions.Compiled);
    public static readonly Regex regVLower = new("в+", RegexOptions.Compiled);
    public static readonly Regex regVUpper = new("В+", RegexOptions.Compiled);
    public static readonly Regex regFLower = new("ф+", RegexOptions.Compiled);
    public static readonly Regex regFUpper = new("Ф+", RegexOptions.Compiled);

    private void OnAccent(EntityUid uid, GrowlingAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        message = regRLower.Replace(message, _random.Pick(new List<string> { "rr", "rrr" }));
        message = regRUpper.Replace(message, _random.Pick(new List<string> { "RR", "RRR" }));
        message = regPLower.Replace(message, _random.Pick(new List<string> { "рр", "ррр" }));
        message = regPUpper.Replace(message, _random.Pick(new List<string> { "РР", "РРР" }));
        message = regVLower.Replace(message, _random.Pick(new List<string> { "вв", "ввв" }));
        message = regVUpper.Replace(message, _random.Pick(new List<string> { "ВВ", "ВВВ" }));
        message = regFLower.Replace(message, _random.Pick(new List<string> { "фф", "ффф" }));
        message = regFUpper.Replace(message, _random.Pick(new List<string> { "ФФ", "ФФФ" }));

        args.Message = message;
    }
    // Adventure zero errors end
}

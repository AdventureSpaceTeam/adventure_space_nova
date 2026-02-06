using System.Text.RegularExpressions;
using Content.Shared.Speech;
using Robust.Shared.Random;

namespace Content.Server._Adventure.DwarfAccent;

public sealed class DwarfAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    // adventure zero warnings begin
    public static readonly Regex regELowerMulti = new("э+", RegexOptions.Compiled);
    public static readonly Regex regEUpperMulti = new("Э+", RegexOptions.Compiled);
    public static readonly Regex regYeLowerMulti = new("е+", RegexOptions.Compiled);
    public static readonly Regex regYeUpperMulti = new("Е+", RegexOptions.Compiled);
    public static readonly Regex regILowerMulti = new("и+", RegexOptions.Compiled);
    public static readonly Regex regIUpperMulti = new("И+", RegexOptions.Compiled);
    // adventure zero warnings end

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DwarfAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, DwarfAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // adventure zero warnings begin
        message = regELowerMulti.Replace(message, _random.Pick(new List<string> { "йе" }));
        message = regEUpperMulti.Replace(message, _random.Pick(new List<string> { "ЙЕ" }));
        message = regYeLowerMulti.Replace(message, _random.Pick(new List<string> { "э" }));
        message = regYeUpperMulti.Replace(message, _random.Pick(new List<string> { "Э" }));
        message = regILowerMulti.Replace(message, _random.Pick(new List<string> { "ые" }));
        message = regIUpperMulti.Replace(message, _random.Pick(new List<string> { "ЫЕ" }));
        // adventure zero warnings end

        args.Message = message;
    }
}

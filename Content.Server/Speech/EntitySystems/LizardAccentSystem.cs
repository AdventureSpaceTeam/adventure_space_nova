using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class LizardAccentSystem : EntitySystem
{
    private static readonly Regex RegexLowerS = new("s+");
    private static readonly Regex RegexUpperS = new("S+");
    private static readonly Regex RegexInternalX = new(@"(\w)x");
    private static readonly Regex RegexLowerEndX = new(@"\bx([\-|r|R]|\b)");
    private static readonly Regex RegexUpperEndX = new(@"\bX([\-|r|R]|\b)");

    // adventure zero warnings begin
    public static readonly Regex regSLowerMulti = new("с+", RegexOptions.Compiled);
    public static readonly Regex regSUpperMulti = new("С+", RegexOptions.Compiled);
    public static readonly Regex regZLowerMulti = new("з+", RegexOptions.Compiled);
    public static readonly Regex regZUpperMulti = new("З+", RegexOptions.Compiled);
    public static readonly Regex regShLower = new("ш+", RegexOptions.Compiled);
    public static readonly Regex regShUpper = new("Ш+", RegexOptions.Compiled);
    public static readonly Regex regChLowerMulti = new("ч+", RegexOptions.Compiled);
    public static readonly Regex regChUpperMulti = new("Ч+", RegexOptions.Compiled);
    public static readonly Regex regZhLowerMulti = new("ж+", RegexOptions.Compiled);
    public static readonly Regex regZhUpperMulti = new("Ж+", RegexOptions.Compiled);
    public static readonly Regex regSchLowerMulti = new("щ+", RegexOptions.Compiled);
    public static readonly Regex regSchUpperMulti = new("Щ+", RegexOptions.Compiled);
    public static readonly Regex regTsLowerMulti = new("ц+", RegexOptions.Compiled);
    public static readonly Regex regTsUpperMulti = new("Ц+", RegexOptions.Compiled);
    // adventure zero warnings end

    [Dependency] private readonly IRobustRandom _random = default!; // Corvax-Localization

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LizardAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, LizardAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // hissss
        message = RegexLowerS.Replace(message, "sss");
        // hiSSS
        message = RegexUpperS.Replace(message, "SSS");
        // ekssit
        message = RegexInternalX.Replace(message, "$1kss");
        // ecks
        message = RegexLowerEndX.Replace(message, "ecks$1");
        // eckS
        message = RegexUpperEndX.Replace(message, "ECKS$1");

        // adventure zero warnings begin
        message = regSLowerMulti.Replace(message, _random.Pick(new List<string>() { "сс", "ссс" }));
        message = regSUpperMulti.Replace(message, _random.Pick(new List<string>() { "СС", "ССС" }));
        message = regZLowerMulti.Replace(message, _random.Pick(new List<string>() { "сс", "ссс" }));
        message = regZUpperMulti.Replace(message, _random.Pick(new List<string>() { "СС", "ССС" }));
        message = regShLower.Replace(message, _random.Pick(new List<string>() { "шш", "шшш", "щщ", "щщщ" }));
        message = regShUpper.Replace(message, _random.Pick(new List<string>() { "ШШ", "ШШШ", "ЩЩ", "ЩЩЩ" }));
        message = regChLowerMulti.Replace(message, _random.Pick(new List<string>() { "щщ", "щщщ", "шш", "шшш" }));
        message = regChUpperMulti.Replace(message, _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ", "ШШ", "ШШШ" }));
        message = regZhLowerMulti.Replace(message, _random.Pick(new List<string>() { "шш", "шшш", "щщ", "щщщ" }));
        message = regZhUpperMulti.Replace(message, _random.Pick(new List<string>() { "ШШ", "ШШШ", "ЩЩ", "ЩЩЩ" }));
        message = regSchLowerMulti.Replace(message, _random.Pick(new List<string>() { "щщ", "щщщ", "шш", "шшш" }));
        message = regSchUpperMulti.Replace(message, _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ", "ШШ", "ШШШ" }));
        message = regTsLowerMulti.Replace(message, _random.Pick(new List<string> { "сс", "ссс" }));
        message = regTsUpperMulti.Replace(message, _random.Pick(new List<string> { "СС", "ССС" }));
        // adventure zero warnings end

        args.Message = message;
    }
}


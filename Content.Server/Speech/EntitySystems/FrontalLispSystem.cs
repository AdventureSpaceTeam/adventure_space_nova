using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random; // Adventure social anxiety
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class FrontalLispSystem : EntitySystem
{
    // @formatter:off
    private static readonly Regex RegexUpperTh = new(@"[T]+[Ss]+|[S]+[Cc]+(?=[IiEeYy]+)|[C]+(?=[IiEeYy]+)|[P][Ss]+|([S]+[Tt]+|[T]+)(?=[Ii]+[Oo]+[Uu]*[Nn]*)|[C]+[Hh]+(?=[Ii]*[Ee]*)|[Z]+|[S]+|[X]+(?=[Ee]+)");
    private static readonly Regex RegexLowerTh = new(@"[t]+[s]+|[s]+[c]+(?=[iey]+)|[c]+(?=[iey]+)|[p][s]+|([s]+[t]+|[t]+)(?=[i]+[o]+[u]*[n]*)|[c]+[h]+(?=[i]*[e]*)|[z]+|[s]+|[x]+(?=[e]+)");
    private static readonly Regex RegexUpperEcks = new(@"[E]+[Xx]+[Cc]*|[X]+");
    private static readonly Regex RegexLowerEcks = new(@"[e]+[x]+[c]*|[x]+");
    // @formatter:on

    // adventure zero warnings begin
    public static readonly Regex regSLowerSingle = new("с", RegexOptions.Compiled);
    public static readonly Regex regSUpperSingle = new("С", RegexOptions.Compiled);
    public static readonly Regex regChLower = new("ч", RegexOptions.Compiled);
    public static readonly Regex regChUpper = new("Ч", RegexOptions.Compiled);
    public static readonly Regex regTsLower = new("ц", RegexOptions.Compiled);
    public static readonly Regex regTsUpper = new("Ц", RegexOptions.Compiled);
    public static readonly Regex regTLower = new("т", RegexOptions.Compiled);
    public static readonly Regex regTUpper = new("Т", RegexOptions.Compiled);
    public static readonly Regex regZLower = new("з", RegexOptions.Compiled);
    public static readonly Regex regZUpper = new("З", RegexOptions.Compiled);
    public static readonly Regex regSchLower = new("щ", RegexOptions.Compiled);
    public static readonly Regex regSchUpper = new("Щ", RegexOptions.Compiled);
    public static readonly Regex regZhLower = new("ж", RegexOptions.Compiled);
    public static readonly Regex regZhUpper = new("Ж", RegexOptions.Compiled);
    // adventure zero warnings end

    [Dependency] private readonly IRobustRandom _random = default!; // Adventure social anxiety

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FrontalLispComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, FrontalLispComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // handles ts, sc(i|e|y), c(i|e|y), ps, st(io(u|n)), ch(i|e), z, s
        message = RegexUpperTh.Replace(message, "TH");
        message = RegexLowerTh.Replace(message, "th");
        // handles ex(c), x
        message = RegexUpperEcks.Replace(message, "EKTH");
        message = RegexLowerEcks.Replace(message, "ekth");

        // Adventure social anxiety begin
        message = regSLowerSingle.Replace(message, _random.Prob(0.90f) ? "ш" : "с");
        message = regSUpperSingle.Replace(message, _random.Prob(0.90f) ? "Ш" : "С");
        message = regChLower.Replace(message, _random.Prob(0.90f) ? "тьш" : "ч");
        message = regChUpper.Replace(message, _random.Prob(0.90f) ? "ТЬШ" : "Ч");
        message = regTsLower.Replace(message, _random.Prob(0.90f) ? "тс" : "ц");
        message = regTsUpper.Replace(message, _random.Prob(0.90f) ? "ТС" : "Ц");
        message = regTLower.Replace(message, _random.Prob(0.90f) ? "тч" : "т");
        message = regTUpper.Replace(message, _random.Prob(0.90f) ? "ТЧ" : "Т");
        message = regZLower.Replace(message, _random.Prob(0.90f) ? "жь" : "з");
        message = regZUpper.Replace(message, _random.Prob(0.90f) ? "ЖЬ" : "З");
        message = regSchLower.Replace(message, _random.Prob(0.90f) ? "шь" : "щ");
        message = regSchUpper.Replace(message, _random.Prob(0.90f) ? "ШЬ" : "Щ");
        message = regZhLower.Replace(message, _random.Prob(0.90f) ? "щь" : "ж");
        message = regZhUpper.Replace(message, _random.Prob(0.90f) ? "ЩЬ" : "Ж");
        // Adventure social anxiety end

        args.Message = message;
    }
}

using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random; // Adventure social anxiety

namespace Content.Server.Speech.EntitySystems;

public sealed class FrontalLispSystem : EntitySystem
{
    // @formatter:off
    private static readonly Regex RegexUpperTh = new(@"[T]+[Ss]+|[S]+[Cc]+(?=[IiEeYy]+)|[C]+(?=[IiEeYy]+)|[P][Ss]+|([S]+[Tt]+|[T]+)(?=[Ii]+[Oo]+[Uu]*[Nn]*)|[C]+[Hh]+(?=[Ii]*[Ee]*)|[Z]+|[S]+|[X]+(?=[Ee]+)");
    private static readonly Regex RegexLowerTh = new(@"[t]+[s]+|[s]+[c]+(?=[iey]+)|[c]+(?=[iey]+)|[p][s]+|([s]+[t]+|[t]+)(?=[i]+[o]+[u]*[n]*)|[c]+[h]+(?=[i]*[e]*)|[z]+|[s]+|[x]+(?=[e]+)");
    private static readonly Regex RegexUpperEcks = new(@"[E]+[Xx]+[Cc]*|[X]+");
    private static readonly Regex RegexLowerEcks = new(@"[e]+[x]+[c]*|[x]+");
    // @formatter:on

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
        // с - ш
        message = Regex.Replace(message, @"с", _random.Prob(0.90f) ? "ш" : "с");
        message = Regex.Replace(message, @"С", _random.Prob(0.90f) ? "Ш" : "С");
        // ч - тьш
        message = Regex.Replace(message, @"ч", _random.Prob(0.90f) ? "тьш" : "ч");
        message = Regex.Replace(message, @"Ч", _random.Prob(0.90f) ? "ТЬШ" : "Ч");
        // ц - тс
        message = Regex.Replace(message, @"ц", _random.Prob(0.90f) ? "тс" : "ц");
        message = Regex.Replace(message, @"Ц", _random.Prob(0.90f) ? "ТС" : "Ц");
        // т - тч
        message = Regex.Replace(message, @"т", _random.Prob(0.90f) ? "тч" : "т");
        message = Regex.Replace(message, @"Т", _random.Prob(0.90f) ? "ТЧ" : "Т");
        // з - жь
        message = Regex.Replace(message, @"з", _random.Prob(0.90f) ? "жь" : "з");
        message = Regex.Replace(message, @"З", _random.Prob(0.90f) ? "ЖЬ" : "З");
        // щ - шь
        message = Regex.Replace(message, @"щ", _random.Prob(0.90f) ? "шь" : "щ");
        message = Regex.Replace(message, @"Щ", _random.Prob(0.90f) ? "ШЬ" : "Щ");
        // ж - щь
        message = Regex.Replace(message, @"ж", _random.Prob(0.90f) ? "щь" : "ж");
        message = Regex.Replace(message, @"Ж", _random.Prob(0.90f) ? "ЩЬ" : "Ж");
        // Adventure social anxiety end

        args.Message = message;
    }
}

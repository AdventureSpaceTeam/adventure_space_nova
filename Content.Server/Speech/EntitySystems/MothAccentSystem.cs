using System.Text.RegularExpressions;
using Content.Server.Speech.Components;

namespace Content.Server.Speech.EntitySystems;

public sealed class MothAccentSystem : EntitySystem
{
    private static readonly Regex RegexLowerBuzz = new Regex("з{1,3}"); // Adventure
    private static readonly Regex RegexUpperBuzz = new Regex("З{1,3}"); // Adventure
    private static readonly Regex RegexLowerBujj = new Regex("ж{1,3}"); // Adventure
    private static readonly Regex RegexUpperBujj = new Regex("Ж{1,3}"); // Adventure

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MothAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, MothAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // buzzz
        message = RegexLowerBuzz.Replace(message, "ззз"); // Adventure
        // buZZZ
        message = RegexUpperBuzz.Replace(message, "ЗЗЗ"); // Adventure
        // bujjj
        message = RegexLowerBujj.Replace(message, "жжж"); // Adventure
        // buJJJ
        message = RegexUpperBujj.Replace(message, "ЖЖЖ"); // Adventure

        args.Message = message;
    }
}

using Content.Server.RPSX.DarkForces.Narsi.Progress;
using Content.Server.RPSX.GameTicking.Rules.Narsi;
using Content.Server.RPSX.GameTicking.Rules.Ratvar;
using Content.Server.RPSX.GameTicking.Rules.Vampire;
using Content.Shared.Database;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Administration.Systems;

public sealed partial class AdminVerbSystem
{
    [Dependency] private readonly NarsiCultProgressSystem _progressSystem = default!;

    private void AddDarkStationAntags(GetVerbsEvent<Verb> args, ICommonSession player)
    {
        Verb narsiCult = new()
        {
            Text = "Сделать культистом Нар'Си",
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/DarkStation/MainGame/DarkForces/Cult/Icons/cultist.rsi"), "cultist"),
            Act = () =>
            {
                _antag.ForceMakeAntag<NarsiRuleComponent>(player, "NarsiCult");
            },
            Impact = LogImpact.High,
            Message = "Делает цель культистом, также включает режим культа"
        };
        args.Verbs.Add(narsiCult);

        Verb narsiCultLeader = new()
        {
            Text = "Сделать лидером культа Нар'Си",
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/DarkStation/MainGame/DarkForces/Cult/Icons/cultist.rsi"),
                "cultistLeader"),
            Act = () =>
            {
                _antag.ForceMakeAntag<NarsiRuleComponent>(player, "NarsiCult");
                _progressSystem.SetNewCultistLeader(args.Target);
            },
            Impact = LogImpact.High,
            Message = "Делает цель лидером культа, также включает режим культа"
        };
        args.Verbs.Add(narsiCultLeader);

        Verb vampire = new()
        {
            Text = "Сделать вампиром",
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/DarkStation/MainGame/DarkForces/Vampire/icons.rsi"), "vampire"),
            Act = () =>
            {
                _antag.ForceMakeAntag<VampireRuleComponent>(player, "Vampire");
            },
            Impact = LogImpact.High,
            Message = "Делает цель вампиром"
        };
        args.Verbs.Add(vampire);

        Verb ratvar = new()
        {
            Text = "Сделать праведником Ратвара",
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/Misc/job_icons.rsi/HeadRevolutionary.png")),
            Act = () =>
            {
                _antag.ForceMakeAntag<RatvarRuleComponent>(player, "Ratvar");
            },
            Impact = LogImpact.High,
            Message = "Делает цель праведником ратвара.",
        };
        args.Verbs.Add(ratvar);
    }
}

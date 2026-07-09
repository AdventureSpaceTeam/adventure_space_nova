using System.Linq;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Objectives;
using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.RPSX.GameTicking.Rules.Vampire;

public sealed class VampireRuleSystem : GameRuleSystem<VampireRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VampireRuleComponent, ObjectivesTextGetInfoEvent>(OnObjectivesTextGetInfo);
        SubscribeLocalEvent<VampireRuleComponent, ObjectivesTextPrependEvent>(OnTextPrepend);
        SubscribeLocalEvent<VampireRuleComponent, AfterAntagEntitySelectedEvent>(OnAntagSelected);
    }

    private void OnTextPrepend(EntityUid uid, VampireRuleComponent component, ObjectivesTextPrependEvent args)
    {
        args.Text += "\n" + "На станции были вампиры";
    }

    private void OnAntagSelected(EntityUid uid, VampireRuleComponent component, AfterAntagEntitySelectedEvent args)
    {
    }

    private void OnObjectivesTextGetInfo(EntityUid uid, VampireRuleComponent component,
        ref ObjectivesTextGetInfoEvent args)
    {
        args.Minds = _antag.GetAntagMindEntityUids(uid).Select(mindId => (mindId, Comp<MindComponent>(mindId).CharacterName ?? "?")).ToList();
        args.AgentName = "Вампир";
    }
}

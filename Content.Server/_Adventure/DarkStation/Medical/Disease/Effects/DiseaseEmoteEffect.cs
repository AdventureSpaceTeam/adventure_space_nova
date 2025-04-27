using Content.Server.Chat.Systems;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._Adventure.Medical.Disease.Effects;

[DataDefinition]
public sealed partial class DiseaseEmoteEffect : DiseaseEffect
{
    [DataField(required: true)]
    private ProtoId<EmotePrototype> EmoteId;

    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);

        var chatSystem = entityManager.System<ChatSystem>();
        chatSystem.TryEmoteWithChat(target, EmoteId);
    }
}

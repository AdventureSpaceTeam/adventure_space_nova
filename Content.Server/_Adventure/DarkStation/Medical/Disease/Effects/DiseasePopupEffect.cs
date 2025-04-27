using Content.Server.Popups;

namespace Content.Server.AdventureSpace.Medical.Disease.Effects;

[DataDefinition]
public sealed partial class DiseasePopupEffect : DiseaseEffect
{
    [DataField(required: true)]
    public string Text;

    public override void Execute(IEntityManager entityManager, EntityUid target)
    {
        base.Execute(entityManager, target);
        var popupSystem = entityManager.System<PopupSystem>();
        popupSystem.PopupEntity(Loc.GetString(Text), target, target);
    }
}

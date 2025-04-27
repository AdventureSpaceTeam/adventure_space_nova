using Content.Shared._Adventure.Medical.Surgery.Components;
using Content.Shared.Body.Part;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;

namespace Content.Server._Adventure.Medical.Surgery.Body;

public sealed partial class SurgeryBodySystem
{
    private void BodyPartFirstInit(Entity<BodyPartComponent> entity, Entity<HumanoidAppearanceComponent> target)
    {
        if (!TryComp<SurgeryBodyPartComponent>(entity, out var surgeryPart))
            return;

        var visuals = surgeryPart.Visuals;
        if (visuals.Color != null || visuals.OverrideColor)
            return;

        foreach (var (_, childSlot) in entity.Comp.Childs)
        {
            var child = GetEntity(childSlot.BodyPart);
            if (child == null || !TryComp<BodyPartComponent>(child.Value, out var childComp))
                continue;

            BodyPartFirstInit((child.Value, childComp), target);
        }

        surgeryPart.Visuals.Color = target.Comp.SkinColor;
        CopyMarking((entity, surgeryPart), entity, target);
    }

    private void CopyMarking(
        Entity<SurgeryBodyPartComponent> surgeryBodyPart,
        Entity<BodyPartComponent> bodyPart,
        Entity<HumanoidAppearanceComponent> target
    )
    {
        var layer = bodyPart.Comp.ToHumanoidLayers();
        if (layer == null)
            return;

        var subLayers = HumanoidVisualLayersExtension.Sublayers(layer.Value);
        foreach (var sublayer in subLayers)
        {
            var markingCategories = MarkingCategoriesConversion.FromHumanoidVisualLayers(sublayer);
            if (target.Comp.MarkingSet.Markings.TryGetValue(markingCategories, out var markings))
                surgeryBodyPart.Comp.Visuals.Markings[markingCategories] = markings;
        }
    }

    private void BodyPartApplyMarking(Entity<BodyPartComponent> entity, Entity<HumanoidAppearanceComponent> target)
    {
        if (!TryComp<SurgeryBodyPartComponent>(entity, out var surgeryPart))
            return;

        if (surgeryPart.Visuals.SpeciesSprite == null)
            return;

        var layer = entity.Comp.ToHumanoidLayers();
        if (layer == null)
            return;

        target.Comp.CustomBaseLayers[layer.Value] = surgeryPart.Visuals.ToLayerInfo();
        ApplyMarking(layer.Value, (entity, surgeryPart), target);

        foreach (var (_, childSlot) in entity.Comp.Childs)
        {
            var child = GetEntity(childSlot.BodyPart);
            if (child == null || !TryComp<BodyPartComponent>(child.Value, out var childComp))
                continue;

            BodyPartApplyMarking((child.Value, childComp), target);
        }
    }

    private void ApplyMarking(
        HumanoidVisualLayers layer,
        Entity<SurgeryBodyPartComponent> bodyPart,
        Entity<HumanoidAppearanceComponent> target
    )
    {
        if (!ShouldApplyMarking(bodyPart))
            return;

        var targetMarking = target.Comp.MarkingSet;
        var markingsDict = bodyPart.Comp.Visuals.Markings;
        var subLayers = HumanoidVisualLayersExtension.Sublayers(layer);
        foreach (var subLayer in subLayers)
        {
            var category = MarkingCategoriesConversion.FromHumanoidVisualLayers(subLayer);
            if (!markingsDict.TryGetValue(category, out var markings))
                continue;

            targetMarking.RemoveCategory(category);
            var newMarkings = targetMarking.AddCategory(category);
            newMarkings.AddRange(markings);
        }
    }

    private bool ShouldApplyMarking(Entity<SurgeryBodyPartComponent> part)
    {
        return !part.Comp.Visuals.OverrideColor;
    }
}

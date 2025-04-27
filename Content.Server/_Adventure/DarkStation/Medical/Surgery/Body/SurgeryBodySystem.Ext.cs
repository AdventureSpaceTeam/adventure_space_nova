using System.Linq;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;

namespace Content.Server.AdventureSpace.Medical.Surgery.Body;

public sealed partial class SurgeryBodySystem
{
    public IEnumerable<(EntityUid Id, BodyPartComponent BodyPart, T Component)> GetBodyChildren<T>(
        EntityUid? id,
        BodyComponent? body = null,
        BodyPartComponent? bodyPart = null,
        T? targetPart = default) where T : IComponent
    {
        if (id is null
            || !Resolve(id.Value, ref body, false)
            || body.RootContainer.ContainedEntity is null
            || !Resolve(body.RootContainer.ContainedEntity.Value, ref targetPart)
            || !Resolve(body.RootContainer.ContainedEntity.Value, ref bodyPart))
            yield break;

        foreach (var child in GetBodyPartChildren(body.RootContainer.ContainedEntity.Value, bodyPart, targetPart))
        {
            yield return child;
        }
    }

    public IEnumerable<(EntityUid Id, BodyPartComponent BodyPart, T Component)> GetBodyPartChildren<T>(
        EntityUid partId,
        BodyPartComponent? bodyPart = null,
        T? part = default) where T : IComponent
    {
        if (!Resolve(partId, ref part, false) || !Resolve(partId, ref bodyPart, false))
            yield break;

        yield return (partId, bodyPart, part);

        foreach (var containerSlotId in bodyPart.Childs.Keys.Select(SharedBodySystem.GetPartSlotContainerId))
        {
            if (!_container.TryGetContainer(partId, containerSlotId, out var container))
                continue;

            foreach (var containedEnt in container.ContainedEntities)
            {
                if (!TryComp(containedEnt, out BodyPartComponent? childPart))
                    continue;

                if (!TryComp<T>(containedEnt, out var childTPart))
                    continue;

                foreach (var value in GetBodyPartChildren(containedEnt, childPart, childTPart))
                {
                    yield return value;
                }
            }
        }
    }

    public IEnumerable<(EntityUid Id, BodyPartComponent BodyPart, T Component)> GetBodyChildrenOfType<T>(
        EntityUid bodyId,
        BodyPartType type,
        BodyPartSymmetry symmetry,
        BodyComponent? body = null) where T : IComponent
    {
        foreach (var part in GetBodyChildren<T>(bodyId, body))
        {
            if (!TryComp<BodyPartComponent>(part.Id, out var bodyPart))
                continue;

            if (bodyPart.PartType == type && bodyPart.Symmetry == symmetry)
                yield return part;
        }
    }
}

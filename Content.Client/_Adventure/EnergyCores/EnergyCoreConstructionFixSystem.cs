using Content.Shared.Construction.Prototypes;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;
using System.Linq;
using Content.Shared._Adventure.EnergyCores;


namespace Content.Client._Adventure.EnergyCores;

public sealed partial class EnergyCoreConstructionFixSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    private EntityQuery<BroadphaseComponent> _broadphaseQuery;
    private EntityQuery<MapGridComponent> _gridQuery;
    private List<ProtoId<ConstructionPrototype>> _protoIds = new();
    private List<ProtoId<EntityPrototype>> _protoIds1 = new();


    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HandsComponent, InRangeOverrideEvent>(OnConstructionInRange);
    }

    public void OnConstructionInRange(Entity<HandsComponent> ent, ref InRangeOverrideEvent args)
    {

        var targetXform = Transform(args.Target);
        // No cross-grid
        if (targetXform.GridUid != Transform(args.User).GridUid)
        {
            return;
        }
        var targetcoord = targetXform.MapPosition;
        var entitycoord = Transform(args.User).MapPosition;
        var dir = targetcoord.Position - entitycoord.Position;
        var length = dir.Length();
        if (length > 20)
            return;
        var query = _prototypeManager.EnumeratePrototypes<ConstructionPrototype>().ToList();
        var query2 = _prototypeManager.EnumeratePrototypes<EntityPrototype>().ToList();
        foreach (var proto in query)
        {
            if (!proto.ID.Contains("EnergyCore")) continue;
            _protoIds.Add(proto);
        }
        foreach (var proto1 in query2)
        {
            if (!proto1.Components.ContainsKey("EnergyCore"))
                continue;
            _protoIds1.Add(proto1);
        }
        if (!TryComp(args.Target, out MetaDataComponent? meta) && meta != null && meta.EntityPrototype != null &&
            (!_protoIds.Contains(meta.EntityPrototype.ID) || !_protoIds1.Contains(meta.EntityPrototype.ID)))
            return;
        args.InRange = true;
    }
}

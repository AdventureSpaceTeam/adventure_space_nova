using System.Linq;
using Content.Server.AdventureSpace.GameRules.SCP;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;

namespace Content.Server.AdventureSpace.GameTicking.Rules.SCP;

public sealed class SCPRuleSystem : GameRuleSystem<SCPRuleComponent>
{
    [Dependency] private readonly SharedTransformSystem _sharedTransform = default!;

    private readonly Dictionary<string, string> _spawners = new()
    {
        { "SCP173", "MobSCP173" },
        { "SCPSoap", "MobSCPSoap" },
        { "SCP049", "MobSCP049" },
    };

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SCPRuleComponent, AntagSelectEntityEvent>(OnSelectSCPEntity);
    }

    private void OnSelectSCPEntity(Entity<SCPRuleComponent> ent, ref AntagSelectEntityEvent args)
    {
        var prefRole = args.PrefRoles.FirstOrDefault();
        args.Entity = FindSCP(_spawners[prefRole]);
    }

    private EntityUid? FindSCP(string prototype)
    {
        var query = EntityQueryEnumerator<SCPMarkerComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (MetaData(uid).EntityPrototype?.ID == prototype)
                return uid;
        }

        return null;
    }
}

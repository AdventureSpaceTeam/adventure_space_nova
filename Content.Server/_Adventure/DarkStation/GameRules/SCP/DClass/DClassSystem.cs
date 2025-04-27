using Content.Shared.CombatMode.Pacification;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Robust.Shared.Timing;

namespace Content.Server._Adventure.GameRules.SCP.DClass;

public sealed class DClassSystem : EntitySystem
{
    private static readonly string StorageImplantID = "StorageImplant";
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedSubdermalImplantSystem _implants = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DClassComponent, ComponentInit>(OnDClassInit);
    }

    private void OnDClassInit(EntityUid uid, DClassComponent component, ComponentInit args)
    {
        var coords = Transform(uid).Coordinates;
        var implant = Spawn(StorageImplantID, coords);

        if (!TryComp<SubdermalImplantComponent>(implant, out var implantComp))
            return;

        _implants.ForceImplant(uid, implant, implantComp);

        component.PacifiedTime = _gameTiming.CurTime + TimeSpan.FromMinutes(20);

        EnsureComp<PacifiedComponent>(uid);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<DClassComponent>();
        while (query.MoveNext(out var uid, out var dClassComponent))
        {
            if (_gameTiming.CurTime < dClassComponent.PacifiedTime || dClassComponent.PacifiedTime == TimeSpan.Zero)
                continue;

            if (HasComp<PacifiedComponent>(uid))
            {
                RemComp<PacifiedComponent>(uid);
                dClassComponent.PacifiedTime = TimeSpan.Zero;
            }
        }
    }
}

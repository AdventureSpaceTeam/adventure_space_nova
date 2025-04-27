using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Progress;

public sealed partial class NarsiCultProgressSystem : EntitySystem
{
    [ValidatePrototypeId<EntityPrototype>]
    private const string NarsiCultProgressHolder = "NarsiCultProgressHolder";

    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly ObjectivesSystem _objectivesSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private Entity<NarsiCultProgressComponent>? _activeProgress;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NarsiCultProgressComponent, ComponentInit>(OnComponentInit);

        InitializeObjectives();
        InitializeCultists();
        InitializeAbilities();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateObjectives();
    }

    public void CreateProgress()
    {
        var progress = Spawn(NarsiCultProgressHolder);
        var progressComp = EnsureComp<NarsiCultProgressComponent>(progress);

        _activeProgress = (progress, progressComp);
    }

    private void OnComponentInit(EntityUid uid, NarsiCultProgressComponent component, ComponentInit args)
    {
        OnObjectivesInit((uid, component));
    }

    public int GetBloodScore()
    {
        return _activeProgress?.Comp.BloodPoints ?? 0;
    }

    private void SendMessageFromNarsi(Entity<NarsiCultProgressComponent> progress, string message)
    {
        _radio.SendRadioMessage(progress,
            message,
            _prototypeManager.Index<RadioChannelPrototype>("NarsiCult"),
            progress);
    }
}

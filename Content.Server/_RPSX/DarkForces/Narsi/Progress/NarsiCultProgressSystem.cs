using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using System;

namespace Content.Server.RPSX.DarkForces.Narsi.Progress;

public sealed partial class NarsiCultProgressSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly ObjectivesSystem _objectivesSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private const string NarsiCultProgressHolder = "NarsiCultProgressHolder";

    private Entity<NarsiCultProgressComponent>? _activeProgress;

    public override void Initialize()
    {
        base.Initialize();

        InitializeObjectives();
        InitializeCultists();
        InitializeAbilities();
    }

    /*public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateObjectives();
    }*/

    public void CreateProgress()
    {
        var progress = Spawn(NarsiCultProgressHolder);
        var progressComp = EnsureComp<NarsiCultProgressComponent>(progress);

        _activeProgress = (progress, progressComp);
        if (_activeProgress != null)
            _activeProgress.Value.Comp.ObjectivesSpawnTime = _gameTiming.CurTime + TimeSpan.FromSeconds(60);
    }

    public int GetBloodScore()
    {
        return _activeProgress?.Comp.BloodPoints ?? 0;
    }

    private void SendMessageFromNarsi(Entity<NarsiCultProgressComponent> progress, string message)
    {
        _radio.SendRadioMessage(progress, message, _prototypeManager.Index<RadioChannelPrototype>("NarsiCult"), progress);
    }
}

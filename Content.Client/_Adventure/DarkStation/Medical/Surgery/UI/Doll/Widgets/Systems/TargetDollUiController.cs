using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.AdventureSpace.Medical.Surgery;
using Content.Shared.Body.Part;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;
using SecretCCVars = Content.Shared.AdventureSpace.CCVars.SecretCCVars;

namespace Content.Client.AdventureSpace.Medical.Surgery.UI.Doll.Widgets.Systems;

public sealed class TargetDollUiController : UIController, IOnStateEntered<GameplayState>,
    IOnSystemChanged<TargetDollSystem>
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private EntityUid? Player => _playerManager.LocalEntity;

    private TargetDollUI? UI => UIManager.GetActiveUIWidgetOrNull<TargetDollUI>();

    public void OnStateEntered(GameplayState state)
    {
        if (!_cfg.GetCVar(SecretCCVars.IsTargetDollEnabled))
            return;

        SyncTargetPart();
    }

    public void OnSystemLoaded(TargetDollSystem system)
    {
        system.SyncTargetPart += SystemOnSyncTargetPart;
        system.Dispose += ClearAllControls;
    }

    public void OnSystemUnloaded(TargetDollSystem system)
    {
        system.SyncTargetPart -= SystemOnSyncTargetPart;
        system.Dispose -= ClearAllControls;
    }

    public override void Initialize()
    {
        base.Initialize();

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
        gameplayStateLoad.OnScreenUnload += OnScreenUnload;
    }

    private void OnScreenUnload()
    {
        UI?.Clear();
    }

    private void OnScreenLoad()
    {
        if (!_cfg.GetCVar(SecretCCVars.IsTargetDollEnabled))
            return;

        UI?.SetupWidget();
        SyncTargetPart();
    }

    private void ClearAllControls(object? sender, EventArgs eventArgs)
    {
        UI?.Hide();
    }

    private void SystemOnSyncTargetPart(object? sender, (BodyPartType?, BodyPartSymmetry) args)
    {
        if (!_cfg.GetCVar(SecretCCVars.IsTargetDollEnabled))
            return;

        if (sender is not TargetDollSystem)
            return;

        if (!_entityManager.HasComponent<TargetDollComponent>(Player))
        {
            UI?.Hide();
            return;
        }

        UI?.Show();
        UI?.SelectBodyPart(args.Item1, args.Item2);
    }

    private void SyncTargetPart()
    {
        if (!_entityManager.TryGetComponent<TargetDollComponent>(Player, out var targetDoll) ||
            !_cfg.GetCVar(SecretCCVars.IsTargetDollEnabled))
        {
            UI?.Hide();
            return;
        }

        UI?.Show();
        UI?.SelectBodyPart(targetDoll.TargetBodyPart, targetDoll?.BodyPartSymmetry ?? BodyPartSymmetry.None);
    }
}

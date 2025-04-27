using Content.Shared.AdventureSpace.Zombie.Smoker;
using Content.Shared.AdventureSpace.Zombie.Smoker.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Input;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics.Joints;

namespace Content.Client.AdventureSpace.Zombie;

public sealed class ZombieSmokerSystem : SharedZombieSmokerSystem
{
    [Dependency] private readonly InputSystem _input = default!;
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();
        _overlay.AddOverlay(new ZombieSmokerOverlay(EntityManager));
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlay.RemoveOverlay<ZombieSmokerOverlay>();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!Timing.IsFirstTimePredicted)
            return;

        var player = _player.LocalSession?.AttachedEntity;
        if (player == null || !TryComp<ZombieSmokerComponent>(player, out var smoker))
            return;

        if (!TryComp<JointComponent>(player, out var jointComp) ||
            !jointComp.GetJoints.TryGetValue(SmokeCufJoint, out var joint) || joint is not DistanceJoint distance)
            return;

        if (distance.MaxLength <= distance.MinLength)
            return;

        var reelKey = _input.CmdStates.GetState(EngineKeyFunctions.UseSecondary) == BoundKeyState.Down;
        if (smoker.Reeling == reelKey)
            return;

        RaisePredictiveEvent(new ZombieSmokerMoveTargetRequestEvent(reelKey));
    }
}

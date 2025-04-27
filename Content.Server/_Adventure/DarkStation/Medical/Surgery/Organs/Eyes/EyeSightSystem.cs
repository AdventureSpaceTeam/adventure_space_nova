using Content.Shared.Body.Components;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;

namespace Content.Server.AdventureSpace.Medical.Surgery.Organs.Eyes;

public sealed class EyeSightSystem : EntitySystem
{
    [Dependency] private readonly BlindableSystem _sharedBlindingSystem = default!;
    [Dependency] private readonly SurgerySystem _surgerySystem = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    private void HandleSight(EntityUid newEntity, EntityUid oldEntity)
    {
        Log.Debug($"HandleSight {newEntity} {oldEntity}");
        //transfer existing component to organ
        var newSight = EntityManager.EnsureComponent<BlindableComponent>(newEntity);
        var oldSight = EntityManager.EnsureComponent<BlindableComponent>(oldEntity);

        //give new sight all values of old sight
        // _sharedBlindingSystem.TransferBlindness(newSight, oldSight);
        _sharedBlindingSystem.AdjustEyeDamage((newEntity, newSight), 0);
        // _sharedBlindingSystem.AdjustBlindSources(newEntity, 0, newSight);

        var hasOtherEyes = false;
        //check for other eye components on owning body and owning body organs (if old entity has a body)
        if (TryComp<BodyComponent>(oldEntity, out var body))
        {
            if (TryComp<EyeSightComponent>(oldEntity,
                    out var bodyEyes)) //some bodies see through their skin!!! (slimes)
                hasOtherEyes = true;
            else
            {
                var organs = _surgerySystem.GetAllBodyOrgans(oldEntity);
                foreach (var organ in organs)
                {
                    if (TryComp<EyeSightComponent>(organ, out var eyes))
                    {
                        hasOtherEyes = true;
                        break;
                    }
                }
                //TODO should we do this for body parts too? might be a little overpowered but could be funny/interesting
            }
        }

        //if there are no existing eye components for the old entity - set old sight to be blind otherwise leave it as is
        if (!hasOtherEyes && !TryComp<EyeSightComponent>(oldEntity, out var self))
            _sharedBlindingSystem.AdjustEyeDamage((oldEntity, oldSight), 9);
    }
}

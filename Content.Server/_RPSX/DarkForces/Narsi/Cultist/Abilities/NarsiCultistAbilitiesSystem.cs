using Content.Server.RPSX.DarkForces.Narsi.Cultist.Abilities.Prototype;
using Content.Server.RPSX.DarkForces.Narsi.Progress;
using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.RPSX.DarkForces.Narsi.Abilities.Events;
using Content.Shared.RPSX.DarkForces.Narsi.Roles;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Content.Shared.Actions.Components;

namespace Content.Server.RPSX.DarkForces.Narsi.Cultist.Abilities;

public sealed partial class NarsiCultistAbilitiesSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly NarsiCultProgressSystem _progressSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;

    private const string EmpAction = "NarsiCultistEmp";
    private const string FireAction = "NarsiCultistFireArms";
    private const string StealthAction = "NarsiCultistStealth";
    private const string ShadowAction = "NarsiCultistShadow";
    private const string TeleportAction = "NarsiCultistTeleport";
    private const string SilenceAction = "NarsiCultistSilence";
    private const string BlindnessAction = "NarsiCultistBlindness";
    private const string StunAction = "NarsiCultistStun";
    private const string CuffAction = "NarsiCultistCuff";
    private const string GhostWeaponAction = "NarsiCultistGhostWeapon";

    public override void Initialize()
    {
        base.Initialize();

        InitializeEmp();
        InitializeSilence();
        InitializeStealth();
        InitializeBlindness();
        InitializeFireArms();
        InitializeShadow();
        InitializeStun();
        InitializeTeleport();
        InitializeCuff();
        InitializeGhostWeapon();
        InitializeLeaderAbilities();
    }

    private void OnCultistAbility(EntityUid uid, INarsiCultistAbility ability)
    {
        if (ability.Speech == null)
            return;

        _chatSystem.TrySendInGameICMessage(uid, ability.Speech, Shared.Chat.InGameICChatType.Whisper, false);
    }

    public void UpdateAbility(string id)
    {
        var level = _progressSystem.GetAbilityLevel(id);
        var cultists = EntityQueryEnumerator<NarsiCultistComponent>();
        while (cultists.MoveNext(out _, out var component))
        {
            if (!component.Abilities.TryGetValue(id, out var ability))
                continue;

            if (_actionsSystem.GetAction(ability) is not Entity<ActionComponent> action)
                continue;

            switch (level)
            {
                case 1:
                    _actionsSystem.SetUseDelay(action.Owner, action.Comp.UseDelay);
                    break;
                case 2:
                    _actionsSystem.SetUseDelay(action.Owner, action.Comp.UseDelay * 0.9);
                    break;
                default:
                    _actionsSystem.SetUseDelay(action.Owner, action.Comp.UseDelay * 0.8);
                    break;
            }
        }
    }

    public bool TryLearnAbility(EntityUid uid, string id)
    {
        if (!TryComp<NarsiCultistComponent>(uid, out var cultistComponent))
            return false;

        if (cultistComponent.Abilities.ContainsKey(id))
            return false;

        var prototype = _prototypeManager.Index<NarsiAbilityPrototype>(id);

        EntityUid? actionUid = null;
        EntityUid? revertActionUid = null;
        if (!_actionsSystem.AddAction(uid, ref actionUid, prototype.ActionId, uid))
            return false;
        if (prototype.RevertActionId.HasValue && !_actionsSystem.AddAction(uid, ref revertActionUid, prototype.RevertActionId.Value, uid))
            return false;

        cultistComponent.Abilities[id] = actionUid;

        return true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        UpdateStealth();
        UpdateSilence();
        UpdateFireArms();
        UpdateBlindness();
    }
}

using System.Linq;
using Content.Server.AdventureSpace.DarkForces.Narsi.Runes.Components;
using Content.Server.Popups;
using Content.Shared.AdventureSpace.Cult.Runes;
using Content.Shared.AdventureSpace.DarkForces.Narsi.Roles;
using Content.Shared.DoAfter;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server.AdventureSpace.DarkForces.Narsi.Runes;

public sealed partial class NarsiRuneSystem : EntitySystem
{
    private static readonly VerbCategory NarsiCult = new("Культ-Нар'Си",
        "/Textures/Interface/VerbIcons/antag-e_sword-temp.192dpi.png");

    private static readonly SoundSpecifier RuneSound =
        new SoundPathSpecifier("/Audio/DarkStation/Narsi/summon_karp.ogg");

    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookupSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundEndedEvent>(OnRoundEnded);
        SubscribeLocalEvent<NarsiRuneComponent, GetVerbsEvent<Verb>>(OnGetVerbs);

        InitSummoningRune();
        InitSummoningNarsiRune();
        InitReviveRune();
    }

    private void OnGetVerbs(EntityUid uid, NarsiRuneComponent component, GetVerbsEvent<Verb> args)
    {
        if (!args.CanAccess || !args.CanInteract || !HasComp<NarsiCultistComponent>(args.User))
            return;

        if (HasComp<NarsiSpawnRuneComponent>(uid))
        {
            AddVerb("Призвать Нар'Си", () => SpawnNarsi(uid, args.User), args);
            return;
        }

        if (HasComp<NarsiOfferingRuneComponent>(uid))
        {
            AddVerb("Предложить жертву", () => ProcessOfferingRune(uid), args);
            return;
        }

        if (HasComp<NarsiReviveRuneComponent>(uid))
        {
            AddVerb("Воскресить культиста", () => TryToRevive(uid, args.User), args);
            return;
        }

        if (TryComp<NarsiTeleportRuneComponent>(uid, out var teleportRuneComponent))
        {
            AddVerb("Установить тэг", () => ShowPopupTag(args.User, teleportRuneComponent), args);
            AddVerb("Телепортироваться на привязанную руну",
                () => TeleportRuneVerb(uid, teleportRuneComponent.Tag),
                args);
            return;
        }

        if (HasComp<NarsiSummonRuneComponent>(uid))
        {
            AddVerb("Призвать культиста", () => TryToSummond(uid, args.User), args);
        }
    }

    private void OnRoundEnded(RoundEndedEvent ev)
    {
        OnRoundEndedReviveRune();
        OnRoundEndSummoningNarsi();
    }

    private void AddVerb(string text, Action action, GetVerbsEvent<Verb> args)
    {
        Verb verb = new()
        {
            Act = action,
            DoContactInteraction = true,
            Text = text,
            Category = NarsiCult,
        };

        args.Verbs.Add(verb);
    }

    private bool HandleRuneInUse(EntityUid rune)
    {
        var runeState = Comp<NarsiRuneComponent>(rune).RuneState;
        if (runeState != NarsiRuneState.InUse)
            return false;

        _popupSystem.PopupEntity("Руну уже используют", rune, PopupType.Medium);
        return true;
    }

    private void HandleRuneUsed(EntityUid rune, bool inUse)
    {
        if (TryComp<AppearanceComponent>(rune, out var appearance))
            _appearance.SetData(rune,
                SharedNarsiRuneComponent.RuneStatus.Status,
                inUse ? SharedNarsiRuneComponent.RuneState.Active : SharedNarsiRuneComponent.RuneState.Idle,
                appearance);

        Comp<NarsiRuneComponent>(rune).RuneState = inUse ? NarsiRuneState.InUse : NarsiRuneState.Idle;
    }

    private void StartDoAfter(DoAfterArgs doAfterArgs, float movementThreshold = 1.0f)
    {
        doAfterArgs.BreakOnMove = true;
        doAfterArgs.MovementThreshold = movementThreshold;

        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }

    private List<Entity<HumanoidAppearanceComponent>> FindHumanoidsNearRune(EntityUid rune, float range = 0.5f)
    {
        var coords = Transform(rune).Coordinates;
        return _entityLookupSystem.GetEntitiesInRange<HumanoidAppearanceComponent>(coords, range)
            .ToList();
    }

    private List<Entity<NarsiCultistComponent>> FindCultistsNearRune(EntityUid rune, float range = 0.5f)
    {
        var coords = Transform(rune).Coordinates;
        return _entityLookupSystem.GetEntitiesInRange<NarsiCultistComponent>(coords, range)
            .ToList();
    }
}

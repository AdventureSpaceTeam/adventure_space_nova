using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server._Adventure.Medical.Surgery.Components;
using Content.Server._Adventure.Medical.Surgery.Events;
using Content.Server._Adventure.Medical.Surgery.Tools;
using Content.Server._Adventure.Medical.Surgery.Tools.Components;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared._Adventure.Medical.Surgery;
using Content.Shared._Adventure.Medical.Surgery.Components;
using Content.Shared.Bed.Sleep;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Adventure.Medical.Surgery;

public sealed partial class SurgerySystem : EntitySystem
{
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly SharedBodySystem _bodySystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly SleepingSystem _sleepingSystem = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterfaceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SurgeryComponent, GetVerbsEvent<Verb>>(AddSurgeryVerb);

        InitBUI();
        InitializeSurgeryDamage();
        InitializeClamp();
        InitializeRejection();
        InitializeOpened();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _gameTiming.CurTime;

        UpdateClamped(curTime);
        UpdateRejection(curTime);
        UpdateOpened(curTime);

        var query = EntityQueryEnumerator<SurgeryComponent>();
        while (query.MoveNext(out var uid, out var surgery))
        {
            //Bleed Check
            if (surgery.OrganBleeding || surgery.PartBleeding)
            {
                surgery.BleedLastChecked += frameTime;
                if (surgery.BleedLastChecked >= surgery.BleedCheckInterval)
                    CalculateBleed(uid, surgery);
            }

            //check if entity has the forced sleep status effect, if they do set Sedated = true, else false
            //TODO forced sleep =/= sedation, will include a dedicated status effect at some point for non-sleeping sedatives (also narcolepsy should not be as useful as anaesthetic)
            surgery.Sedated = HasComp<ForcedSleepingComponent>(uid);
        }
    }

    /// <summary>
    ///     Handle Bleeding to occur (eventually replace this when either Woundmed or something like it is implemented)
    ///     Routinely run this function via update
    ///     Keep track of surgery incurred bleeding via surgery component
    ///     Ensure bleed is equal to b + s where b is the entity bleed outside of surgery and s is surgery bleed (part or
    ///     organ)
    ///     If the surgery bleeding is stopped and all surgical wounds sealed, reduce bleed by s
    /// </summary>
    private void CalculateBleed(EntityUid uid, SurgeryComponent body)
    {
        //Get entity bleed damage bloodstream component
        if (!TryComp<BloodstreamComponent>(uid, out var bloodstream))
            return;

        //Get last SurgeryBleed value from surgery component
        var currentBleed = body.SurgeryBleed;

        //Negate SurgeryBleed from EntityBleed to determine Bleed independent of surgery (minium 0)
        var nonSurgeryBleed = Math.Max(bloodstream.BleedAmount - body.SurgeryBleed, 0f);

        //Add that value with organ bleed or part bleed, which ever is greater (and applicable)
        var newBleed = nonSurgeryBleed;
        if (body.PartBleeding)
            newBleed += body.BasePartBleed;
        else if (body.OrganBleeding)
            newBleed += body.BaseOrganBleed;


        //Modify Bleed for bloodstream component to be equal to that value
        if (_bloodstreamSystem.TryModifyBleedAmount(uid, newBleed - bloodstream.BleedAmount, bloodstream))
            body.SurgeryBleed = newBleed;

        //The result should ensure that entity bleeding is allowed to occur normally while surgery bleed is continuously maintained
        //Entity bleed heals over time on its own or via other methods, but surgery bleed can only be stopped by direct surgical interaction - effectively being constant
        //But once addressed, surgery bleed is taken away immediately
    }

    /// <summary>
    ///     Handles status effects such as bleeding and opened for when a part/organ/slot is affected
    ///     Applies "shock" (airloss) damage is the body is not sedated
    /// </summary>
    public void SetBodyStatusFromChange(EntityUid uid, SurgeryToolUsage toolUsage)
    {
        //get surgery component
        if (!TryComp<SurgeryComponent>(uid, out var surgery))
            return;

        var toolAppliedEvent = new SurgeryToolAppliedEvent(toolUsage);
        RaiseLocalEvent(uid, toolAppliedEvent);

        //get all entity body part slots and organ slots (and constituent parts and organs)
        var bodyPartSlots = GetAllBodyPartSlots(uid);
        var organSlots = GetOpenPartOrganSlots(bodyPartSlots);

        if (!TryComp<BloodstreamComponent>(uid, out var bloodstream))
            return;

        //check clamp/cauterised/occupied status of all slots - if any are empty and not cauterised/clamped set either part or organ bleeding to true
        var partNotClamped = false;
        var currentPartBleed = surgery.PartBleeding;
        var currentOrganBleed = surgery.OrganBleeding;
        foreach (var slot in bodyPartSlots)
        {
            if (slot.BodyPart != null || slot.Cauterised ||
                _toolsSystem.BodyPartSlotHasAttachment<SurgeryLargeClampComponent>(slot))
                continue;

            partNotClamped = true;
            break;
        }

        surgery.PartBleeding = partNotClamped;
        //if the patient has gone from not bleeding to bleeding, run an initial bloodloss
        if (!currentPartBleed && surgery.PartBleeding)
        {
            //if the part bleed is new but there was a bleeding organ, only make up the difference
            if (!currentOrganBleed)
                _bloodstreamSystem.TryModifyBloodLevel(uid, -surgery.InitialPartBloodloss, bloodstream);
            else
            {
                _bloodstreamSystem.TryModifyBloodLevel(uid,
                    -Math.Abs(surgery.InitialPartBloodloss - surgery.InitialOrganBloodloss),
                    bloodstream);
            }
        }

        var organNotClamped = false;

        foreach (var slot in organSlots)
        {
            if (slot.Child != null || slot.Cauterised ||
                _toolsSystem.OrganSlotHasAttachment<SurgerySmallClampComponent>(slot))
                continue;

            organNotClamped = true;
            break;
        }

        surgery.OrganBleeding = organNotClamped;
        //if the patient has gone from not bleeding to bleeding, run an initial bloodloss
        if (!currentPartBleed && !currentOrganBleed && surgery.OrganBleeding)
            _bloodstreamSystem.TryModifyBloodLevel(uid, -surgery.InitialOrganBloodloss, bloodstream);
    }

    /// <summary>
    ///     Get body part slots for a body part (usually starting with then torso, then followed by limbs)
    /// </summary>
    private List<BodyPartSlot> GetBodyPartSlots(EntityUid? bodyPart)
    {
        var bodyPartSlots = new List<BodyPartSlot>();

        if (TryComp<BodyPartComponent>(bodyPart, out var bodyPartComp))
            bodyPartSlots.AddRange(bodyPartComp.Childs.Select(partSlot => partSlot.Value));

        return bodyPartSlots;
    }

    /// <summary>
    ///     Get all body part slots attached to everybody part attached to the initally submitted part (usually the torso to
    ///     start)
    /// </summary>
    private List<BodyPartSlot> GetAllBodyPartSlots(EntityUid bodyOwner)
    {
        EntityUid? rootPart; // = body.Root.Child;

        //using body uid, get root part slot's child (usually the torso)
        if (TryComp<BodyComponent>(bodyOwner, out var body))
        {
            if (body.RootContainer.ContainedEntity == null)
                return new List<BodyPartSlot>();

            rootPart = body.RootContainer.ContainedEntity;

            if (rootPart == null)
                return new List<BodyPartSlot>();
        }
        else if (TryComp<BodyPartComponent>(bodyOwner, out var bodyPart))
            rootPart = bodyOwner;
        else
            return new List<BodyPartSlot>();

        //proceed to get all part slots
        var initialPartList = GetBodyPartSlots(rootPart);
        var additionalPartList = new List<BodyPartSlot>();
        //then check all parts from that
        foreach (var t in initialPartList)
        {
            void RecursiveGetSlots(BodyPartSlot bodyPartSlot)
            {
                if (bodyPartSlot.BodyPart == null)
                    return;

                var subPartList = GetBodyPartSlots(GetEntity(bodyPartSlot.BodyPart));
                foreach (var subSlot in subPartList)
                {
                    RecursiveGetSlots(subSlot);
                }

                additionalPartList.AddRange(subPartList);
            }

            RecursiveGetSlots(t);
        }

        initialPartList.AddRange(additionalPartList);

        if (TryComp<BodyPartComponent>(rootPart, out var bodyPartComp) && bodyPartComp.BodyPartSlot != null)
            initialPartList.Add(bodyPartComp.BodyPartSlot);
        else if (bodyPartComp != null)
        {
            var tempSelfSlot = new BodyPartSlot("self",
                GetNetEntity(rootPart.Value),
                bodyPartComp.PartType,
                BodyPartSymmetry.None)
            {
                BodyPart = GetNetEntity(rootPart),
            };
            initialPartList.Add(tempSelfSlot);
        }

        return initialPartList;
    }

    /// <summary>
    ///     Check all submitted body parts, check if they are opened and if they are get all organ slots
    /// </summary>
    private List<OrganSlot> GetOpenPartOrganSlots(List<BodyPartSlot> bodyPartSlots)
    {
        var organSlots = new List<OrganSlot>();

        foreach (var t in bodyPartSlots)
        {
            if (t.BodyPart is null)
                continue;

            var part = GetEntity(t.BodyPart.Value);
            if (!TryComp<BodyPartComponent>(part, out var bodyPart))
                continue;

            if (!TryComp<SurgeryBodyPartComponent>(part, out var surgeryPart))
                continue;

            organSlots.AddRange(from entry in bodyPart.Organs
                where surgeryPart.State.Opened || !entry.Value.Internal
                select entry.Value);
        }

        return organSlots;
    }

    private List<OrganSlot> GetAllPartOrganSlots(List<BodyPartSlot> bodyPartSlots)
    {
        var organSlots = new List<OrganSlot>();

        foreach (var t in bodyPartSlots)
        {
            if (t.BodyPart is not null && TryComp<BodyPartComponent>(GetEntity(t.BodyPart), out var bodyPart))
                organSlots.AddRange(bodyPart.Organs.Select(entry => entry.Value));
        }

        return organSlots;
    }

    /// <summary>
    ///     Get all organs in a body, useful for checking if a body has a certain organ or for any kind of body scanners
    /// </summary>
    public List<EntityUid> GetAllBodyOrgans(EntityUid uid)
    {
        var bodyPartSlots = GetAllBodyPartSlots(uid);
        var organSlots = GetAllPartOrganSlots(bodyPartSlots);

        return organSlots.Where(slot => slot.Child != null).Select(slot => GetEntity(slot.Child!.Value)).ToList();
    }

    /// <summary>
    ///     Opens main surgery interface
    /// </summary>
    private void StartOpeningSurgery(EntityUid user, EntityUid target, bool openInCombat = false)
    {
        if (TryComp<CombatModeComponent>(user, out var mode) && mode.IsInCombatMode && !openInCombat)
            return;

        _userInterfaceSystem.TryOpenUi(target, SurgeryUiKey.Key, user);
        UpdateUiState(target);
    }

    private void AddSurgeryVerb(EntityUid uid, SurgeryComponent component, GetVerbsEvent<Verb> args)
    {
        if (args.Hands == null || !args.CanAccess || !args.CanInteract)
            return;

        if (!HasComp<ActorComponent>(args.User))
            return;

        Verb verb = new()
        {
            Icon = new SpriteSpecifier.Rsi(new ResPath("Objects/Specific/Medical/Surgery/scalpel.rsi"), "scalpel"),
            Text = Loc.GetString("surgery-perform"),
            Act = () => StartOpeningSurgery(args.User, uid, true),
        };
        args.Verbs.Add(verb);
    }

    private bool GetBodyFromPart(EntityUid uid, [NotNullWhen(true)] out EntityUid? body)
    {
        body = null;

        if (TryComp<BodyPartComponent>(uid, out var bodyPart) && bodyPart.Body != null)
        {
            body = bodyPart.Body;
            return true;
        }

        if (TryComp<OrganComponent>(uid, out var organ) && organ.Body != null)
        {
            body = organ.Body;
            return true;
        }

        return false;
    }
}

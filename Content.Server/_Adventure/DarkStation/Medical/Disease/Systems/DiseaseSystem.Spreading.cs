using Content.Server.AdventureSpace.Medical.Disease.Components;
using Content.Server.AdventureSpace.Medical.Disease.Data;
using Content.Server.Body.Systems;
using Content.Shared.Interaction.Events;

namespace Content.Server.AdventureSpace.Medical.Disease.Systems;

public sealed partial class DiseaseSystem
{
    private const int DiseaseExhaleSpreadRange = 2;
    [Dependency] private readonly DiseaseProtectionClothingSystem _diseaseProtection = default!;

    private void InitializeSpreading()
    {
        SubscribeLocalEvent<DiseaseSpreaderComponent, ExhaleLocationEvent>(OnDiseaseSpreaderExhale);
        SubscribeLocalEvent<DiseaseSpreaderComponent, ContactInteractionEvent>(OnSpreaderContact);
    }

    private void OnSpreaderContact(Entity<DiseaseSpreaderComponent> ent, ref ContactInteractionEvent args)
    {
        var target = args.Other;
        if (!TryComp<DiseaseTargetComponent>(target, out var diseaseTarget))
            return;

        if (_diseaseProtection.IsClothingProtectFromSpreading(ent, DiseaseSpreading.Contact) ||
            _diseaseProtection.IsClothingProtectFromSpreading(ent, DiseaseSpreading.Contact))
            return;

        ent.Comp.Diseases.SpreadDisease(
            (target, diseaseTarget),
            DiseaseSpreading.Contact,
            AddDisease
        );
    }


    private void OnDiseaseSpreaderExhale(EntityUid uid, DiseaseSpreaderComponent component, ExhaleLocationEvent args)
    {
        if (component.Diseases.Count == 0)
            return;

        if (_diseaseProtection.IsClothingProtectFromSpreading(uid, DiseaseSpreading.Air))
            return;

        var targetCoords = Transform(uid);
        var entities = _entityLookupSystem.GetEntitiesInRange<DiseaseTargetComponent>(
            targetCoords.Coordinates,
            DiseaseExhaleSpreadRange
        );

        component.Diseases.SpreadDisease(
            entities,
            DiseaseSpreading.Air,
            (entity, key) =>
            {
                if (_diseaseProtection.IsClothingProtectFromSpreading(entity, DiseaseSpreading.Air))
                    return;

                AddDisease(entity, key);
            }
        );
    }

    public void TransferDiseasesContact(EntityUid recipient, EntityUid donor)
    {
        if (!TryComp<DiseaseTargetComponent>(donor, out var diseaseTarget))
            return;

        var spreaderComp = EnsureComp<DiseaseSpreaderComponent>(recipient);
        foreach (var (key, data) in diseaseTarget.Diseases)
        {
            if (data.Spreads is not { } spreads)
                continue;

            if (spreads.Contains(DiseaseSpreading.Contact))
                spreaderComp.Diseases.SaveAddDisease(DiseaseSpreading.Contact, key);
        }

        foreach (var diseaseId in spreaderComp.Diseases[DiseaseSpreading.Contact])
        {
            AddDisease(donor, diseaseId);
        }
    }

    private void SetupSpreading(Entity<DiseaseTargetComponent> target, string disease, List<DiseaseSpreading>? spreads)
    {
        if (spreads == null)
            return;

        var spreading = EnsureComp<DiseaseSpreaderComponent>(target);
        foreach (var spread in spreads)
        {
            spreading.Diseases.SaveAddDisease(spread, disease);
        }
    }
}

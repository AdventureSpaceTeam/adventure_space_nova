using Content.Server.Body.Events;

namespace Content.Server._Adventure.Medical.Surgery.Organs.Kidney;

public sealed class KidneySystem : SurgeryOrganSystem<ToxinRemoverComponent, ToxinRemoverMarkerComponent>
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ToxinRemoverComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<ToxinRemoverComponent, OnEntityMetabolize>(OnMetabolize);
        SubscribeLocalEvent<ToxinRemoverComponent, OnEntityMetabolizeAfterReagent>(OnMetabolizeAfterReagent);
    }

    private void OnMetabolize(Entity<ToxinRemoverComponent> ent, ref OnEntityMetabolize args)
    {
        float toxinRemovalRate;
        if (!ent.Comp.RemoverToxins.Contains(args.Entry.Id))
            return;

        if (ent.Comp.Working)
        {
            //determine if there is enough build up to diminish the remover's effectiveness
            var toxinRemovalPenalty = 0f;
            if (ent.Comp.Damage >= ent.Comp.WarningThreshold)
                toxinRemovalPenalty = ent.Comp.Damage * ent.Comp.BuildUpRemovalMod -
                                      ent.Comp.WarningThreshold * ent.Comp.BuildUpRemovalMod;

            toxinRemovalRate = ent.Comp.ToxinRemovalRate - toxinRemovalPenalty;
            //always maintain a minimum removal rate
            if (toxinRemovalRate < ent.Comp.BaseToxinRemovalRate)
                toxinRemovalRate = ent.Comp.BaseToxinRemovalRate;
        }
        else
            toxinRemovalRate = ent.Comp.BaseToxinRemovalRate;

        args.MostToRemove *= toxinRemovalRate;
    }

    private void OnMetabolizeAfterReagent(Entity<ToxinRemoverComponent> ent, ref OnEntityMetabolizeAfterReagent args)
    {
        if (!ent.Comp.RemoverToxins.Contains(args.Entry.Id))
            return;

        ent.Comp.Damage += args.Scale;
        UpdateOrganStatus(ent);
    }

    private void OnComponentInit(EntityUid uid, ToxinRemoverComponent component, ComponentInit args)
    {
        component.WarningThreshold = component.MaxDamageThreshold * 0.5f;
        component.CriticalThreshold = component.MaxDamageThreshold * 0.75f;
    }
}

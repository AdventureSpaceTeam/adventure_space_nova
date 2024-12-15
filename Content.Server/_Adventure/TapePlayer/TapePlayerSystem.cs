using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction.Events;
using Content.Shared.TapePlayer;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;

namespace Content.Server._Adventure.TapePlayer;
public sealed class TapePlayerSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;

    private readonly string itemSlotName = "tape";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TapePlayerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<TapePlayerComponent, UseInHandEvent>(OnActivate);
        SubscribeLocalEvent<TapePlayerComponent, EntRemovedFromContainerMessage>(OnItemRemoved);
    }

    private void OnActivate(EntityUid uid, TapePlayerComponent comp, UseInHandEvent args)
    {
        var tape = comp.TapeSlot.Item;
        if (tape == null)
            return;
        if (!TryComp<MusicTapeComponent>(tape, out var musicTapeComponent))
            return;
        if (musicTapeComponent.Sound == null)
            return;

        if (comp.Played)
        {
            _audioSystem.Stop(comp.AudioStream);
            comp.Played = false;
            return;
        }

        comp.Played = true;
        var param = AudioParams.Default.WithLoop(true)
            .WithVolume(comp.Volume)
            .WithMaxDistance(comp.MaxDistance)
            .WithRolloffFactor(comp.RolloffFactor);
        var stream = _audioSystem.PlayPvs(musicTapeComponent.Sound, uid, param);
        comp.AudioStream = stream.Value.Entity;
    }

    private void OnItemRemoved(Entity<TapePlayerComponent> ent, EntRemovedFromContainerMessage args)
    {
        _audioSystem.Stop(Comp.AudioStream);
        ent.Comp.Played = false;
    }

    private void OnComponentInit(Entity<TapePlayerComponent> ent, ref ComponentInit args)
    {
        if (!TryGetSlot(ent, itemSlotName, out ent.Comp.TapeSlot))
            _itemSlotsSystem.AddItemSlot(ent, itemSlotName, ent.Comp.TapeSlot);
    }
}

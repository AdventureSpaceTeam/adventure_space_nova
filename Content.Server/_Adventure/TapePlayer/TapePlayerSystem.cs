using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction.Events;
using Content.Shared._Adventure.TapePlayer;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;

namespace Content.Server._Adventure.TapePlayer;
public sealed class TapePlayerSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ItemSlotsSystem _item = default!;

    private readonly string itemSlotName = "tape";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TapePlayerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<TapePlayerComponent, UseInHandEvent>(OnActivate);
        SubscribeLocalEvent<TapePlayerComponent, EntRemovedFromContainerMessage>(OnItemRemoved);
    }

    private void OnActivate(Entity<TapePlayerComponent> ent, ref UseInHandEvent args)
    {
        var tape = ent.Comp.TapeSlot.Item;
        if (tape == null)
            return;
        if (!TryComp<MusicTapeComponent>(tape, out var musicTapeComponent))
            return;
        if (musicTapeComponent.Sound == null)
            return;

        if (ent.Comp.Played)
        {
            _audio.Stop(ent.Comp.AudioStream);
            ent.Comp.Played = false;
            return;
        }

        ent.Comp.Played = true;
        var param = AudioParams.Default.WithLoop(true)
            .WithVolume(ent.Comp.Volume)
            .WithMaxDistance(ent.Comp.MaxDistance)
            .WithRolloffFactor(ent.Comp.RolloffFactor);
        var stream = _audio.PlayPvs(musicTapeComponent.Sound, ent, param);
        if (stream == null)
            return;
        ent.Comp.AudioStream = stream.Value.Entity;
    }

    private void OnItemRemoved(Entity<TapePlayerComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        _audio.Stop(ent.Comp.AudioStream);
        ent.Comp.Played = false;
    }

    private void OnComponentInit(Entity<TapePlayerComponent> ent, ref ComponentInit args)
    {
        _item.AddItemSlot(ent, itemSlotName, ent.Comp.TapeSlot);
    }
}

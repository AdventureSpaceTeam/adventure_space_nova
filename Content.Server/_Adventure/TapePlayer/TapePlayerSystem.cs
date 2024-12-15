using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction.Events;
using Content.Shared.Interaction;
using Content.Shared._Adventure.TapePlayer;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Audio;
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
        SubscribeLocalEvent<TapePlayerComponent, UseInHandEvent>(OnHandActivate);
        SubscribeLocalEvent<TapePlayerComponent, ActivateInWorldEvent>(OnWorldActivate);
        SubscribeLocalEvent<TapePlayerComponent, EntRemovedFromContainerMessage>(OnItemRemoved);
    }

    private void OnActivate(Entity<TapePlayerComponent> ent)
    {
        var tapeEnt = _item.GetItemOrNull(ent, itemSlotName);
        if (tapeEnt == null)
            return;
        if (!TryComp<MusicTapeComponent>(tapeEnt, out var tape))
            return;
        if (tape.Sound == null)
            return;

        if (_audio.IsPlaying(ent.Comp.AudioStream))
        {
            _audio.Stop(ent.Comp.AudioStream);
            return;
        }

        var param = AudioParams.Default.WithLoop(true)
            .WithVolume(ent.Comp.Volume)
            .WithMaxDistance(ent.Comp.MaxDistance)
            .WithRolloffFactor(ent.Comp.RolloffFactor);
        var stream = _audio.PlayPvs(tape.Sound, ent, param);
        if (stream == null)
            return;
        ent.Comp.AudioStream = stream.Value.Entity;
    }

    private void OnHandActivate(Entity<TapePlayerComponent> ent, ref UseInHandEvent args)
    {
        OnActivate(ent);
    }

    private void OnWorldActivate(Entity<TapePlayerComponent> ent, ref ActivateInWorldEvent args)
    {
        OnActivate(ent);
    }

    private void OnItemRemoved(Entity<TapePlayerComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        _audio.Stop(ent.Comp.AudioStream);
        ent.Comp.Played = false;
    }
}

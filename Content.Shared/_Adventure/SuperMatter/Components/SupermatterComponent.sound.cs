using Robust.Shared.Audio;

namespace Content.Shared._Adventure.Supermatter.Components;

public sealed partial class SupermatterComponent
{
    /// <summary>
    /// Current stream of SM audio.
    /// </summary>
    public EntityUid? AudioStream;

    public SuperMatterSound? SmSound;

    [DataField("dustSound")]
    public SoundSpecifier DustSound = new SoundPathSpecifier("/Audio/_Adventure/Supermatter/dust.ogg");

    [DataField("delamSound")]
    public SoundSpecifier DelamSound = new SoundPathSpecifier("/Audio/_Adventure/Supermatter/delamming.ogg");

    [DataField("delamAlarm")]
    public SoundSpecifier DelamAlarm = new SoundPathSpecifier("/Audio/Machines/alarm.ogg");
}

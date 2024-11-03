using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Adventure.ACVar;

/// <summary>
///     Adventure's config vars.
/// </summary>
[CVarDefs]
// ReSharper disable once InconsistentNaming
public sealed class ACVars : CVars
{
    // ReSharper disable once InconsistentNaming
    [CVarDefs]
    public sealed class ACVars : CVars
    {
        /// <summary>
        ///     The token used to authenticate with Discord. For the Bot to function set: discord.token, discord.guild_id, and discord.prefix.
        ///     If this is empty, the bot will not connect.
        /// </summary>
        public static readonly CVarDef<string> DiscordToken =
            CVarDef.Create("discord.token", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

        /// <summary>
        ///     The Discord guild ID to use for commands as well as for several other features, like the ahelp relay.
        ///     If this is empty, the bot will not connect.
        /// </summary>
        public static readonly CVarDef<string> DiscordGuildId =
            CVarDef.Create("discord.guild_id", string.Empty, CVar.SERVERONLY);

        /// <summary>
        ///     Prefix used for commands for the Discord bot.
        ///     If this is empty, the bot will not connect.
        /// </summary>
        public static readonly CVarDef<string> DiscordPrefix =
            CVarDef.Create("discord.prefix", "!", CVar.SERVERONLY);
        /// <summary>
        ///     The discord channel id that discord messages are sent to.
        /// </summary>
        public static readonly CVarDef<string> AdminRelayChannelId =
            CVarDef.Create("discord.admin_chat_relay_channel_id", string.Empty, CVar.SERVERONLY);
        /// <summary>
        ///     The discord **FORUM** channel id that discord messages are sent to. If it's not a forum channel, everything will explode.
        /// </summary>
        public static readonly CVarDef<string> AhelpRelayChannelId =
            CVarDef.Create("discord.ahelp_relay_channel_id", string.Empty, CVar.SERVERONLY);
        ///     If this is true, the ahelp relay shows that the response was from discord. If this is false, all messages an discord sends will be shown as if the discord was ingame.
        /// </summary>
        public static readonly CVarDef<bool> AhelpRelayShowDiscord =
            CVarDef.Create("discord.ahelp_relay_show_discord", false, CVar.SERVERONLY);
        /// <summary>
        /// The discord channel id that OOC messages are sent to.
        /// </summary>
        public static readonly CVarDef<string> OocRelayChannelId =
            CVarDef.Create("discord.ooc_relay_channel_id", "", CVar.SERVERONLY);

    }

    /**
     * TTS (Text-To-Speech)
     */

    /// <summary>
    /// URL of the TTS server API.
    /// </summary>
    public static readonly CVarDef<bool> TTSEnabled =
        CVarDef.Create("tts.enabled", false, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

    /// <summary>
    /// URL of the TTS server API.
    /// </summary>
    public static readonly CVarDef<string> TTSApiUrl =
        CVarDef.Create("tts.api_url", "", CVar.SERVERONLY | CVar.ARCHIVE);

    /// <summary>
    /// Auth token of the TTS server API.
    /// </summary>
    public static readonly CVarDef<string> TTSApiToken =
        CVarDef.Create("tts.api_token", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Amount of seconds before timeout for API
    /// </summary>
    public static readonly CVarDef<int> TTSApiTimeout =
        CVarDef.Create("tts.api_timeout", 5, CVar.SERVERONLY | CVar.ARCHIVE);

    /// <summary>
    /// Default volume setting of TTS sound
    /// </summary>
    public static readonly CVarDef<float> TTSVolume =
        CVarDef.Create("tts.volume", 0f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Default volume setting of TTS Radio sound
    /// </summary>
    public static readonly CVarDef<float> TTSRadioVolume =
        CVarDef.Create("tts.radio_volume", 0f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Count of in-memory cached tts voice lines.
    /// </summary>
    public static readonly CVarDef<int> TTSMaxCache =
        CVarDef.Create("tts.max_cache", 250, CVar.SERVERONLY | CVar.ARCHIVE);

    /// <summary>
    /// Tts rate limit values are accounted in periods of this size (seconds).
    /// After the period has passed, the count resets.
    /// </summary>
    public static readonly CVarDef<float> TTSRateLimitPeriod =
        CVarDef.Create("tts.rate_limit_period", 2f, CVar.SERVERONLY);

    /// <summary>
    /// How many tts preview messages are allowed in a single rate limit period.
    /// </summary>
    public static readonly CVarDef<int> TTSRateLimitCount =
        CVarDef.Create("tts.rate_limit_count", 3, CVar.SERVERONLY);

    /// <summary>
    /// TTS request timeout in seconds.
    /// </summary>
    public static readonly CVarDef<float> TTSRequestTimeout =
        CVarDef.Create("tts.timeout", 5f, CVar.SERVERONLY | CVar.ARCHIVE);

    /// <summary>
    /// VoiceId for Announcement TTS
    /// </summary>
    public static readonly CVarDef<string> TTSAnnounceVoiceId =
        CVarDef.Create("tts.announce_voice", "Skippy", CVar.SERVERONLY | CVar.ARCHIVE);

    /// <summary>
    /// Default volume setting of TTS Announce sound
    /// </summary>
    public static readonly CVarDef<float> TTSAnnounceVolume =
        CVarDef.Create("tts.announce_volume", 0f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Enable client TTS subscription
    /// </summary>
    public static readonly CVarDef<bool> TTSClientEnabled =
        CVarDef.Create("tts.client_enabled", true, CVar.CLIENTONLY | CVar.ARCHIVE);


    /*
     * Sponsor API
     */

    public static readonly CVarDef<string> SponsorApiUrl =
        CVarDef.Create("sponsor.api_url", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> SponsorApiToken =
        CVarDef.Create("sponsor.api_token", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}

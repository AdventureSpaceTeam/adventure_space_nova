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

    /// <summary>
    /// Start map and preset votes in lobby automatically.
    /// </summary>
    public static readonly CVarDef<bool> LobbyVote =
        CVarDef.Create("game.lobby_vote", true, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

    /*
     * Mentor
     */

    /// <summary>
    /// Working only for mentors, always played when player is not mentor.
    /// </summary>
    public static readonly CVarDef<bool> MentorHelpSoundEnabled =
        CVarDef.Create("mentor.sound_enabled", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    /*
     * Discord
     */

    /// <summary>
    /// URL of the discord webhook to relay bans messages.
    /// </summary>
    public static readonly CVarDef<string> DiscordBanWebhook =
        CVarDef.Create("discord.ban_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /*
     * Discord sponsors
     */

    public static readonly CVarDef<string> DiscordSponsorsGuildId =
        CVarDef.Create("discord_sponsors.guild_id", string.Empty, CVar.SERVERONLY);

    public static readonly CVarDef<string> DiscordSponsorsBotToken =
        CVarDef.Create("discord_sponsors.bot_token", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /*
     * Discord auth
     */

    public static readonly CVarDef<bool> DiscordAuthEnabled =
        CVarDef.Create("discord_auth.enabled", false, CVar.SERVERONLY);

    // Doesn't hot-reload, you need to restart server when this value changes.
    public static readonly CVarDef<string> DiscordAuthListeningUrl =
        CVarDef.Create("discord_auth.listening_url", "http://localhost:3963/", CVar.SERVERONLY);

    public static readonly CVarDef<string> DiscordAuthRedirectUrl =
        CVarDef.Create("discord_auth.redirect_url", "http://localhost:3963/", CVar.SERVERONLY);

    public static readonly CVarDef<string> DiscordAuthClientId =
        CVarDef.Create("discord_auth.client_id", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<string> DiscordAuthClientSecret =
        CVarDef.Create("discord_auth.client_secret", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<string> DiscordAuthDebugApiUrl =
        CVarDef.Create("discord_auth.debug_api_url", "https://discord.com/api/v10", CVar.SERVERONLY);
}

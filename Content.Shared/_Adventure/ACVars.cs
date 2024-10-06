using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Adventure.ACVar
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
}

using Content.Server.Discord;
using System.Threading.Tasks;
using Robust.Shared.IoC;
using Content.Shared._Adventure.ACVar;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Content.Shared.Database;
using Content.Server.Administration;
using Content.Server.GameTicking;

namespace Content.Server._Adventure.Discord;

public sealed class DiscordWebhookBanSender
{   
    [Dependency] private readonly DiscordWebhook _discord = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IEntitySystemManager _entSys = default!;

    private ISawmill _sawmill = default!;

    private readonly string _iconUrl = "https://cdn.discordapp.com/emojis/1167383322863878214.webp?size=44";
    private readonly string _thumbnailIconUrl ="https://static.wikia.nocookie.net/ss14andromeda13/images/f/ff/Clown.png/revision/latest?cb=20230217121049&path-prefix=ru";   

    public async void SendBanMessage(string? targetUsername, string? banningAdmin, uint minutes, string reason)
    {
        try
        {
            var gameTicker = _entSys.GetEntitySystemOrNull<GameTicker>();
            var serverName = _cfg.GetCVar(CCVars.GameHostName);

            var runId = gameTicker != null ? gameTicker.RoundId : 0;

            var expired = minutes != 0 ? $"{DateTimeOffset.Now + TimeSpan.FromMinutes(minutes)}" : "Никогда";

            var payload = new WebhookPayload()
            {
                Username = "Банановые острова",
                Embeds = new List<WebhookEmbed>
                    {
                        new()
                        {
                            Title = minutes != 0 ? $"<:TeamAdmin:1198975655715553310> Временный бан на {minutes} мин" : "<:TeamAdmin:1198975655715553310> Перманентная блокировка",
                            Color = minutes != 0 ? 16600397 : 13438992,
                            Description =  $"""
                                > **Администратор**
                                > **Логин:** {banningAdmin ?? "Консоль"}

                                > **Нарушитель**
                                > **Логин:** {targetUsername ?? "Неизвестно"}
                                                               
                                > **Номер раунда:** {runId}
                                > **Выдан:** {DateTimeOffset.Now}
                                > **Истекает:** {expired}

                                > **Причина:** {reason}
                                """,
                            Footer = new WebhookEmbedFooter
                            {
                                Text = serverName,
                                IconUrl = _iconUrl
                            },
                            Thumbnail = new WebhookEmbedThumbnail
                            {
                                Url = _thumbnailIconUrl
                            }
                        },
                    },
            };

            var webhookUrl = _cfg.GetCVar(ACVars.DiscordBanWebhook);

            if (string.IsNullOrEmpty(webhookUrl))
                return;

            if (await _discord.GetWebhook(webhookUrl) is not { } identifier)
                return;

            var request = await _discord.CreateMessage(identifier.ToIdentifier(), payload);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Error while sending ban webhook to Discord: {e}");
        }
    }

    public async void SendRoleBansMessage(string? targetUsername, string? banningAdmin, uint minutes, string reason, IReadOnlyCollection<string>? roles)
    {
        try
        {
            var gameTicker = _entSys.GetEntitySystemOrNull<GameTicker>();
            var serverName = _cfg.GetCVar(CCVars.GameHostName);

            var runId = gameTicker != null ? gameTicker.RoundId : 0;

            var expired = minutes != 0 ? $"{DateTimeOffset.Now + TimeSpan.FromMinutes(minutes)}" : "Никогда";

            string formattedRolesStr = "";

            foreach (var role in roles!){
            formattedRolesStr = formattedRolesStr + $"\n> `{Loc.GetString($"Job{role}")}`";
            }
            
            var payload = new WebhookPayload()
            {
                Username = "Банановые острова",
                Embeds = new List<WebhookEmbed>
                    {
                        new()
                        {
                            Title = minutes != 0 ? $"<:TeamMod:1198975738376892476>  Временный джоб-бан на {minutes} мин" : "<:TeamMod:1198975738376892476> Перманентный джоб-бан",
                            Color = minutes != 0 ? 28927 : 2815,
                            Description =  $"""
                                > **Администратор**
                                > **Логин:** {banningAdmin ?? "Консоль"}

                                > **Нарушитель**
                                > **Логин:** {targetUsername ?? "Неизвестно"}

                                > **Номер раунда:** {runId}
                                > **Выдан:** {DateTimeOffset.Now}
                                > **Истекает:** {expired}

                                > **Роли:** {formattedRolesStr}

                                > **Причина:** {reason}
                                """,
                            Footer = new WebhookEmbedFooter
                            {
                                Text = serverName,
                                IconUrl = _iconUrl
                            },
                            Thumbnail = new WebhookEmbedThumbnail
                            {
                                Url = _thumbnailIconUrl
                            }
                        },
                    },
            };

            var webhookUrl = _cfg.GetCVar(ACVars.DiscordBanWebhook);

            if (string.IsNullOrEmpty(webhookUrl))
                return;

            if (await _discord.GetWebhook(webhookUrl) is not { } identifier)
                return;

            var request = await _discord.CreateMessage(identifier.ToIdentifier(), payload);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Error while sending ban webhook to Discord: {e}");
        }
    }    
}
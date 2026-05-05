using Content.Server._Adventure.DiscordAuth; // adventure discord auth
using Content.Server._Adventure.TTS; // c4llv07e tts
using Content.Server._Adventure.Sponsors; // c4llv07e sponsors
using Content.Shared._Adventure.Sponsors; // c4llv07e sponsors
using Content.Server._RMC14.Mentor; // taken from RMC-14
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Administration.Notes;
using Content.Server.Afk;
using Content.Server.Chat.Managers;
using Content.Server.Connection;
using Content.Server.Database;
using Content.Server.Discord;
using Content.Server.Discord.DiscordLink;
using Content.Server.Discord.WebhookMessages;
using Content.Server.EUI;
using Content.Server.GhostKick;
using Content.Server.Info;
using Content.Server.Mapping;
using Content.Server.Maps;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.Players.JobWhitelist;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Players.RateLimiting;
using Content.Server.Preferences.Managers;
using Content.Server.ServerInfo;
using Content.Server.ServerUpdates;
using Content.Server.Voting.Managers;
using Content.Server.Worldgen.Tools;
using Content.Shared.Administration.Logs;
using Content.Shared.Administration.Managers;
using Content.Shared.Chat;
using Content.Shared.IoC;
using Content.Shared.Kitchen;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Players.RateLimiting;
using Content.Server._Adventure; // AdvSpace Config
using Content.Server._Adventure.Discord; // AdvSpace Discord Webhook

namespace Content.Server.IoC;

internal static class ServerContentIoC
{
    public static void Register(IDependencyCollection deps)
    {
        SharedContentIoC.Register(deps);
        deps.Register<DiscordAuthBotManager>(); // adventure discord auth
        deps.Register<TTSManager>(); // c4llv07e tts
        deps.Register<ISponsorsManager, SponsorsManager>(); // c4llv07e sponsors
        deps.Register<SponsorsManager>(); // c4llv07e sponsors
        deps.Register<AdventureConfigManager>(); // AdvSpace Config
        deps.Register<AdventurePresetManager>(); // AdvSpace Preset
        deps.Register<DiscordWebhookBanSender>(); // AdvSpace Discord Webhook
        deps.Register<MentorManager>(); // Adventure mentor help
        deps.Register<IChatManager, ChatManager>();
        deps.Register<ISharedChatManager, ChatManager>();
        deps.Register<IChatSanitizationManager, ChatSanitizationManager>();
        deps.Register<IServerPreferencesManager, ServerPreferencesManager>();
        deps.Register<IServerDbManager, ServerDbManager>();
        deps.Register<RecipeManager, RecipeManager>();
        deps.Register<INodeGroupFactory, NodeGroupFactory>();
        deps.Register<IConnectionManager, ConnectionManager>();
        deps.Register<ServerUpdateManager>();
        deps.Register<IAdminManager, AdminManager>();
        deps.Register<ISharedAdminManager, AdminManager>();
        deps.Register<EuiManager, EuiManager>();
        deps.Register<IVoteManager, VoteManager>();
        deps.Register<IPlayerLocator, PlayerLocator>();
        deps.Register<IAfkManager, AfkManager>();
        deps.Register<IGameMapManager, GameMapManager>();
        deps.Register<RulesManager, RulesManager>();
        deps.Register<IBanManager, BanManager>();
        deps.Register<ContentNetworkResourceManager>();
        deps.Register<IAdminNotesManager, AdminNotesManager>();
        deps.Register<GhostKickManager>();
        deps.Register<ISharedAdminLogManager, AdminLogManager>();
        deps.Register<IAdminLogManager, AdminLogManager>();
        deps.Register<PlayTimeTrackingManager>();
        deps.Register<UserDbDataManager>();
        deps.Register<ServerInfoManager>();
        deps.Register<PoissonDiskSampler>();
        deps.Register<DiscordWebhook>();
        deps.Register<VoteWebhooks>();
        deps.Register<ServerDbEntryManager>();
        deps.Register<ISharedPlaytimeManager, PlayTimeTrackingManager>();
        deps.Register<ServerApi>();
        deps.Register<JobWhitelistManager>();
        deps.Register<PlayerRateLimitManager>();
        deps.Register<SharedPlayerRateLimitManager, PlayerRateLimitManager>();
        deps.Register<MappingManager>();
        deps.Register<IWatchlistWebhookManager, WatchlistWebhookManager>();
        deps.Register<ConnectionManager>();
        deps.Register<MultiServerKickManager>();
        deps.Register<CVarControlManager>();
        deps.Register<DiscordLink>();
        deps.Register<DiscordChatLink>();
    }
}

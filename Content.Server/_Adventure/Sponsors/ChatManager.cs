using Content.Server._Adventure.Sponsors;

namespace Content.Server.Chat.Managers;

internal sealed partial class ChatManager : IChatManager
{
    [Dependency] private readonly SponsorsManager _sponsors = default!;
}

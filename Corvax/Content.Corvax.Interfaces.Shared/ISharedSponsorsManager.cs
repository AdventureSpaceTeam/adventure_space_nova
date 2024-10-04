using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Corvax.Interfaces.Shared;

public interface ISharedSponsorsManager
{
    public void Initialize();

    // Client
    public List<string> GetClientPrototypes();
    public bool GetClientAllowedRespawn();

    // Server
    public int GetServerExtraCharSlots(NetUserId userId);
    public bool HaveServerPriorityJoin(NetUserId userId);
    public bool IsSponsor(NetUserId userId);
    public NetUserId PickRoleSession(HashSet<NetUserId> users, string roleId);
    public bool GetServerAllowedRespawn(NetUserId userId);
    public bool TryGetServerPrototypes(NetUserId userId, [NotNullWhen(true)] out List<string>? prototypes);
    public bool TryGetServerOocColor(NetUserId userId, [NotNullWhen(true)] out Color? color);
    public bool TryGetServerOocTitle(NetUserId userId, [NotNullWhen(true)] out string? title);
    public bool TryGetServerGhostTheme(NetUserId userId, [NotNullWhen(true)] out string? ghostTheme);
}


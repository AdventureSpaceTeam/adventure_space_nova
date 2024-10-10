using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Alteros.Interfaces.Shared;

public sealed class SharedSponsorsManager
{
    public void Initialize()
    {
    }

    public List<string> GetClientPrototypes()
    {
        return new List<string>();
    }

    public bool ClientAllowedRespawn()
    {
        return false;
    }

    public bool TryGetPrototypes(NetUserId userId, [NotNullWhen(true)] out List<string>? prototypes)
    {
        prototypes = null;
        return false;
    }

    public bool TryGetOocTitle(NetUserId userId, [NotNullWhen(true)] out string? title)
    {
        title = null;
        return false;
    }

    public bool TryGetOocColor(NetUserId userId, [NotNullWhen(true)] out Color? color)
    {
        color = null;
        return false;
    }

    public bool TryGetGhostTheme(NetUserId userId, [NotNullWhen(true)] out string? ghostTheme)
    {
        ghostTheme = null;
        return false;
    }

    public int GetExtraCharSlots(NetUserId userId)
    {
        return 0;
    }

    public bool HavePriorityJoin(NetUserId userId)
    {
        return false;
    }

    public bool IsSponsor(NetUserId userId)
    {
        return false;
    }

    public bool IsAllowedRespawn(NetUserId userId)
    {
        return false;
    }

    public List<ICommonSession> PickPrioritySessions(List<ICommonSession> sessions, string roleId)
    {
        return new List<ICommonSession>();
    }

    public NetUserId? PickRoleSession(HashSet<NetUserId> users, string roleId)
    {
        return null;
    }

    public bool TryGetPriorityGhostRoles(NetUserId userId, [NotNullWhen(true)] out List<string>? priorityAntags)
    {
        priorityAntags = null;
        return false;
    }

    public bool TryGetPriorityAntags(NetUserId userId, [NotNullWhen(true)] out List<string>? priorityAntags)
    {
        priorityAntags = null;
        return false;
    }

    public bool TryGetPriorityRoles(NetUserId userId, [NotNullWhen(true)] out List<string>? priorityRoles)
    {
        priorityRoles = null;
        return false;
    }
}

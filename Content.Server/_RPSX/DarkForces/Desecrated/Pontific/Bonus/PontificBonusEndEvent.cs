using Robust.Shared.GameObjects;

namespace Content.Server.RPSX.DarkForces.Desecrated.Pontific.Bonus;

public sealed class PontificBonusEndEvent : EntityEventArgs
{
    public PontificBonusEndEvent(string key)
    {
        Key = key;
    }

    public string Key;
}

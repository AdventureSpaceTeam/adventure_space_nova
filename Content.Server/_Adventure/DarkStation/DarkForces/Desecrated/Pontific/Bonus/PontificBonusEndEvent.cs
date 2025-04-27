namespace Content.Server.AdventureSpace.DarkForces.Desecrated.Pontific.Bonus;

public sealed class PontificBonusEndEvent : EntityEventArgs
{
    public string Key;

    public PontificBonusEndEvent(string key)
    {
        Key = key;
    }
}

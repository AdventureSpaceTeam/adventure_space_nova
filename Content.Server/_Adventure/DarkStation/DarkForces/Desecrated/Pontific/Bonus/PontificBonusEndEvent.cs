namespace Content.Server._Adventure.DarkForces.Desecrated.Pontific.Bonus;

public sealed class PontificBonusEndEvent : EntityEventArgs
{
    public string Key;

    public PontificBonusEndEvent(string key)
    {
        Key = key;
    }
}

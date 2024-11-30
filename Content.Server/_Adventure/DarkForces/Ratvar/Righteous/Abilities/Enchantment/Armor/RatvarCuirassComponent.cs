﻿namespace Content.Server.AdventurePrivate._Alteros.DarkForces.Ratvar.Righteous.Abilities.Enchantment.Armor;

[RegisterComponent]
public sealed partial class RatvarCuirassComponent : Component
{
    [DataField]
    public int AbsorbCount;

    [DataField]
    public bool IsAbsorb;

    [DataField]
    public bool IsHardenPlates;

    [DataField]
    public bool IsReflection;
}

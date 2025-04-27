namespace Content.Server.AdventureSpace.DarkForces.Ratvar.Righteous.Abilities.Enchantment.Items;

[RegisterComponent]
public sealed partial class RatvarShardComponent : Component
{
    [DataField]
    public float ConvertRange = 3f;

    [DataField]
    public string TileId = "FloorBrass";
}

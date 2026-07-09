using Content.Shared.Materials;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.RPSX.DarkForces.Ratvar.Righteous.Structures.Workshop;

[RegisterComponent]
public sealed partial class RatvarworkShopComponent : Component
{
    [DataField]
    public bool InProgress;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<MaterialPrototype>))]
    public string RequiredMaterial = "BrassPlasteel";
}

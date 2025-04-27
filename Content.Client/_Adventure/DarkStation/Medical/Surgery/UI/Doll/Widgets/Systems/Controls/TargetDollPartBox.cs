using Content.Shared.Body.Part;

namespace Content.Client._Adventure.Medical.Surgery.UI.Doll.Widgets.Systems.Controls;

public sealed class TargetDollPartBox
{
    private string _texturePath = string.Empty;

    public Box2 Pos;
    public BodyPartSymmetry Symmetry;
    public BodyPartType? Type;

    public TargetDollPartBox(string texturePath,
        Box2 pos,
        BodyPartType? type = null,
        BodyPartSymmetry symmetry = BodyPartSymmetry.None)
    {
        TexturePath = texturePath;
        Pos = pos;
        Type = type;
        Symmetry = symmetry;
    }

    public string TexturePath
    {
        set => _texturePath = "/Textures/DarkStation/MainGame/Interface/Default/TargetDoll/" + value;
        get => _texturePath;
    }
}

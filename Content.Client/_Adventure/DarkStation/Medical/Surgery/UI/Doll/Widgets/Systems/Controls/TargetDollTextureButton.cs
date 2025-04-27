using System.Linq;
using System.Numerics;
using Content.Shared.Body.Part;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Adventure.Medical.Surgery.UI.Doll.Widgets.Systems.Controls;

public sealed class TargetDollTextureButton : TextureButton
{
    private readonly List<TargetDollPartBox> _bodyParts = new()
    {
        new TargetDollPartBox(
            "surgery_deactivated.png",
            Box2.Empty
        ),
        new TargetDollPartBox(
            "surgery_arm_left.png",
            new Box2(new Vector2(80, 86), new Vector2(97, 54)),
            BodyPartType.Arm,
            BodyPartSymmetry.Left
        ),
        new TargetDollPartBox(
            "surgery_arm_right.png",
            new Box2(new Vector2(30, 45), new Vector2(40, 84)),
            BodyPartType.Arm,
            BodyPartSymmetry.Right
        ),
        new TargetDollPartBox(
            "surgery_chest.png",
            new Box2(new Vector2(47, 85), new Vector2(73, 43)),
            BodyPartType.Torso
        ),
        new TargetDollPartBox(
            "surgery_head.png",
            new Box2(new Vector2(45, 35), new Vector2(74, 14)),
            BodyPartType.Head
        ),
        new TargetDollPartBox(
            "surgery_leg_left.png",
            new Box2(new Vector2(63, 124), new Vector2(87, 94)),
            BodyPartType.Leg,
            BodyPartSymmetry.Left
        ),
        new TargetDollPartBox(
            "surgery_leg_right.png",
            new Box2(new Vector2(34, 123), new Vector2(57, 94)),
            BodyPartType.Leg,
            BodyPartSymmetry.Right
        ),
    };

    private TargetDollPartBox? _selectedPart;

    public EventHandler<(BodyPartType?, BodyPartSymmetry)>? OnTargetBodyPartChanged;

    public TargetDollTextureButton()
    {
        SetSize = new Vector2(128, 128);
        HorizontalAlignment = HAlignment.Left;
        VerticalAlignment = VAlignment.Top;
        OnButtonUp += SelectPart;
    }

    private void SelectPart(ButtonEventArgs obj)
    {
        var pos = obj.Event.RelativePosition;
        foreach (var part in _bodyParts.Where(part => part.Pos.Contains(pos)))
        {
            if (part == _selectedPart)
            {
                SelectPart();
                OnTargetBodyPartChanged?.Invoke(this, (null, BodyPartSymmetry.None));
                break;
            }

            _selectedPart = part;
            SelectPart(part.Type, part.Symmetry);
            OnTargetBodyPartChanged?.Invoke(this, (part.Type, part.Symmetry));
            break;
        }
    }

    public void SelectPart(BodyPartType? type = null, BodyPartSymmetry symmetry = BodyPartSymmetry.None)
    {
        var targetPart = _bodyParts.Find(part => part.Type == type && part.Symmetry == symmetry);
        if (targetPart == null)
        {
            TexturePath = _bodyParts.First().TexturePath;
            return;
        }

        _selectedPart = targetPart;
        TexturePath = targetPart.TexturePath;
    }
}

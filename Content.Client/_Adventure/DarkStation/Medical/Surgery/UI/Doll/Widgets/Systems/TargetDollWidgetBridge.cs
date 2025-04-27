using Content.Client._c4llv07e.Bridges;
using Content.Client.AdventureSpace.Medical.Surgery.UI.Doll.Widgets.Systems.Controls;
using Content.Shared.Body.Part;
using Robust.Client.UserInterface;

namespace Content.Client.AdventureSpace.Medical.Surgery.UI.Doll.Widgets.Systems;

public sealed class TargetDollWidgetBridge : ITargetDollWidgetBridge
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    private TargetDollTextureButton? _dollTextureRect;
    private Control? _surface;

    public void InitializeWidget()
    {
        IoCManager.InjectDependencies(this);
    }

    public void SetupWidget(Control surface)
    {
        if (_surface != null)
            Clear();

        var dollTexture = new TargetDollTextureButton();
        dollTexture.OnTargetBodyPartChanged += OnTargetBodyPartChanged;

        surface.AddChild(dollTexture);

        _surface = surface;
        _dollTextureRect = dollTexture;
        _surface = surface;
        _dollTextureRect = dollTexture;
    }

    public void SelectBodyPart(BodyPartType? targetBodyPart, BodyPartSymmetry bodyPartSymmetry)
    {
        _dollTextureRect?.SelectPart(targetBodyPart, bodyPartSymmetry);
    }

    public void Clear()
    {
        _surface?.RemoveAllChildren();
        _surface = null;

        _dollTextureRect = null;
        _dollTextureRect?.RemoveAllChildren();
    }

    public void Hide()
    {
        if (_surface != null)
            _surface.Visible = false;
    }

    public void Show()
    {
        if (_surface != null)
            _surface.Visible = true;
    }

    private void OnTargetBodyPartChanged(object? sender, (BodyPartType?, BodyPartSymmetry) e)
    {
        _entityManager.System<TargetDollSystem>().OnTagetBodyPartChanged(e.Item1, e.Item2);
    }
}

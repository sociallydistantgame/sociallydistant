using AcidicGUI.Rendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI.Shell;

public sealed class DockIconView : Widget
{
    private readonly Button              button = new();
    private readonly CompositeIconWidget icon   = new();
    private          bool                isActive;
    private          Action?             clickHandler;
    
    public DockIconView()
    {
        Children.Add(button);
        button.Content = icon;

        button.Margin = 3;
        
        button.Clicked += OnClick;
    }

    private void OnClick()
    {
        clickHandler?.Invoke();
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        var color = GetVisualStyle().SelectionColor;

        if (isActive)
        {
            geometry.AddRoundedRectangle(ContentArea, 3, color);
        }
    }

    public void UpdateView(DockGroup.IconDefinition definition)
    {
        isActive = definition.IsActive;
        icon.Icon = definition.Icon;
        clickHandler = definition.ClickHandler;

        InvalidateGeometry();
    }
}
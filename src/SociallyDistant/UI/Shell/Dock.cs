using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.Shell;

public sealed class Dock : Widget
{
    private readonly Box root = new();
    private readonly FlexPanel dockItems = new();

    public Dock()
    {
        MinimumSize = new Point(27, 0);
        
        root.Margin = 3;
        dockItems.Spacing = 3;
        
        Children.Add(root);
        root.Content = dockItems;

        root.SetCustomProperty(WidgetBackgrounds.Dock);
    }
}
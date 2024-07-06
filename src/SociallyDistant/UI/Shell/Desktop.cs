using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Architecture;

namespace SociallyDistant.UI.Shell;

public class Desktop : 
    Widget,
    IDisposable
{
    private readonly FlexPanel root = new();
    private readonly Dock dock = new();
    private readonly DesktopController desktopController;

    internal Desktop(DesktopController desktopController)
    {
        this.desktopController = desktopController;
        root.Padding = 3;
        root.Spacing = 3;
        root.Direction = Direction.Horizontal;
        
        Children.Add(root);

        root.ChildWidgets.Add(dock);
    }
    
    public void Dispose()
    {
    }
}
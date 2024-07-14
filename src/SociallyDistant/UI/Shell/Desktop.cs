using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Architecture;

namespace SociallyDistant.UI.Shell;

public class Desktop : 
    Widget,
    IDisposable
{
    private readonly FlexPanel         root = new();
    private readonly Dock              dock;
    private readonly Box               toolsSlot = new();
    private readonly Box               infoSlot  = new();
    private readonly DesktopController desktopController;

    internal Desktop(DesktopController desktopController)
    {
        dock = new Dock(desktopController);
        
        this.desktopController = desktopController;
        root.Padding = 3;
        root.Spacing = 3;
        root.Direction = Direction.Horizontal;
        
        Children.Add(root);

        root.ChildWidgets.Add(dock);
        root.ChildWidgets.Add(toolsSlot);
        root.ChildWidgets.Add(infoSlot);

        toolsSlot.Content = desktopController.ToolsRootWidget;
    }
    
    public void Dispose()
    {
        dock.Dispose();
        
        toolsSlot.Content = null;
        infoSlot.Content = null;
    }
}
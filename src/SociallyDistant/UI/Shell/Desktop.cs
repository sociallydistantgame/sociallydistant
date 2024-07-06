using AcidicGUI.Layout;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.Shell;

public class Desktop : 
    Widget,
    IDisposable
{
    private readonly FlexPanel root = new();
    private readonly Dock dock = new();

    public Desktop()
    {
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
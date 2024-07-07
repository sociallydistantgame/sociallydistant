using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.Windowing;

public class OverlayWorkspace :
    Widget,
    IClientWorkspaceDefinition<Window, Widget?>
{
    private readonly FloatingWorkspace workspace = new();

    public OverlayWorkspace()
    {
        Children.Add(workspace);
        this.SetCustomProperty(WidgetBackgrounds.Overlay);
    }
    
    public Window CreateWindow(string title, Widget? client = default)
    {
        return workspace.CreateWindow(title, client);
    }

    public IReadOnlyList<IWindow> WindowList => workspace.WindowList;
    public IFloatingGui CreateFloatingGui(string title)
    {
        throw new NotImplementedException();
    }
}
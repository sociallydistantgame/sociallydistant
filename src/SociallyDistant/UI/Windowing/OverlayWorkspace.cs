using AcidicGUI.Events;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Effects;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.Windowing;

public class OverlayWorkspace :
    Widget,
    IClientWorkspaceDefinition<Window, Widget?>,
    IMouseDownHandler
{
    private readonly FloatingWorkspace workspace = new();

    public OverlayWorkspace()
    {
        Children.Add(workspace);
        this.SetCustomProperty(WidgetBackgrounds.Overlay);
        this.RenderEffect = BackgroundBlurWidgetEffect.GetEffect(Application.Instance.Context);
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

    public void OnMouseDown(MouseButtonEvent e)
    {
        e.RequestFocus();
    }
}
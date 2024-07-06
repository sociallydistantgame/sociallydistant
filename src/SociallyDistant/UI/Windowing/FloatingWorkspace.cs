using AcidicGUI;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
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

public class FloatingWorkspace :
    Widget,
    IClientWorkspaceDefinition<Window, Widget?>,
    INotifyCloseWorkspace
{
    private readonly List<WindowBase> windows = new();

    public FloatingWorkspace()
    {
        LayoutRoot = this;
    }
    
    public Window CreateWindow(string title, Widget? client = default)
    {
        var win = new Window(this);

        win.SetClient(client);
        
        this.Children.Add(win);
        windows.Add(win);

        return win;
    }

    public void OnCloseWindow(WindowBase window)
    {
        windows.Remove(window);
        Children.Remove(window);
    }

    public IReadOnlyList<IWindow> WindowList => windows;
    public IFloatingGui CreateFloatingGui(string title)
    {
        throw new NotImplementedException();
    }

    public IMessageDialog CreateMessageDialog(string title)
    {
        throw new NotImplementedException();
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        return availableSize;
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        foreach (Widget win in Children)
        {
            var childSize = win.GetCachedContentSize(availableSpace.Size);
            var windowSettings = win.GetCustomProperties<WindowSettings>();

            if (windowSettings.Maximized)
            {
                win.UpdateLayout(context, availableSpace);
            }
            else
            {
                var positionInWorkspace = availableSpace.Center + windowSettings.Position;
                var layoutPosition = positionInWorkspace - (childSize / 2);

                win.UpdateLayout(context, new LayoutRect(
                    layoutPosition.X,
                    layoutPosition.Y,
                    childSize.X,
                    childSize.Y
                ));
            }


        }
    }
}

public sealed class WindowSettings : CustomPropertyObject
{
    private Vector2 position;
    private bool maximized;

    public bool Maximized
    {
        get => maximized;
        set
        {
            maximized = value;
            Widget.InvalidateLayout();
        }
    }
    
    public Vector2 Position
    {
        get => position;
        set
        {
            position = value;
            Widget.InvalidateLayout();
        }
    }

    public WindowSettings(Widget owner) : base(owner)
    {
    }
}
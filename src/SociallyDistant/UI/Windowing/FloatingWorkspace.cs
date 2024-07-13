using AcidicGUI;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

public class FloatingWorkspace :
    Widget,
    IClientWorkspaceDefinition<Window, Widget?>,
    INotifyCloseWorkspace
{
    private readonly List<WindowBase> windows = new();
    private bool maximizeAll;

    public bool MaximizeAll
    {
        get => maximizeAll;
        set
        {
            maximizeAll = value;
            InvalidateLayout();
        }
    }
    
    public FloatingWorkspace()
    {
        LayoutRoot = this;
    }
    
    public Window CreateWindow(string title, Widget? client = default)
    {
        var win = new Window(this);

        win.Title = title;
        
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
            var windowSettings = win.GetCustomProperties<WindowSettings>();
            var size = windowSettings.Size;

            if (windowSettings.Maximized || maximizeAll)
                size = availableSpace.Size;
            
            var childSize = win.GetCachedContentSize(size);

            if (windowSettings.Maximized || maximizeAll)
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

    public TabbedWindow CreateTabbedWindow()
    {
        var win = new TabbedWindow(this);
        
        windows.Add(win);
        Children.Add(win);

        return win;
    }
}

public sealed class WindowSettings : CustomPropertyObject
{
    private Vector2 position;
    private Vector2 size;
    private bool    maximized;

    public bool Maximized
    {
        get => maximized;
        set
        {
            maximized = value;
            Widget.InvalidateLayout();
        }
    }

    public Vector2 Size
    {
        get => size;
        set
        {
            size = value;
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